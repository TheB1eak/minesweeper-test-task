using Minesweeper.Core.Components;
using Minesweeper.Core.Services;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Minesweeper.Core.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ProcessRevealCommandsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonEntity<GameSessionTag>(out Entity sessionEntity))
                return;

            GameSessionState session = SystemAPI.GetComponent<GameSessionState>(sessionEntity);
            if (session.Status != GameStatus.Playing)
                return;

            if (!SystemAPI.TryGetSingletonBuffer<RevealCellCommand>(out DynamicBuffer<RevealCellCommand> revealCommands))
                return;

            if (revealCommands.Length == 0)
                return;

            EcsServices services = SystemAPI.ManagedAPI.GetSingleton<EcsServices>();
            IGridLookup gridLookup = services.GridLookup;
            EntityManager entityManager = state.EntityManager;

            NativeArray<RevealCellCommand> commands =
                new NativeArray<RevealCellCommand>(revealCommands.Length, Allocator.Temp);

            for (int i = 0; i < revealCommands.Length; i++)
                commands[i] = revealCommands[i];

            revealCommands.Clear();

            if (!session.MinesPlaced && services.Settings.SafeFirstClick)
            {
                for (int i = 0; i < commands.Length; i++)
                {
                    int2 position = commands[i].Position;
                    if (!gridLookup.TryGetEntity(position.x, position.y, out Entity entity))
                        continue;

                    CellState cell = entityManager.GetComponentData<CellState>(entity);
                    if (CellFlags.Has(cell.Flags, CellFlags.Open) || CellFlags.Has(cell.Flags, CellFlags.Flag))
                        continue;

                    MinePlacer.PlaceMines(
                        entityManager,
                        gridLookup,
                        session.MineCount,
                        (uint)math.hash(position),
                        position.x,
                        position.y);

                    AdjacentMineCalculator.Calculate(entityManager, gridLookup);
                    session.MinesPlaced = true;
                    break;
                }
            }

            NativeQueue<int2> revealQueue = new NativeQueue<int2>(Allocator.Temp);

            for (int i = 0; i < commands.Length; i++)
            {
                int2 position = commands[i].Position;
                if (!gridLookup.TryGetEntity(position.x, position.y, out Entity entity))
                    continue;

                CellState cell = entityManager.GetComponentData<CellState>(entity);
                if (CellFlags.Has(cell.Flags, CellFlags.Open) || CellFlags.Has(cell.Flags, CellFlags.Flag))
                    continue;

                revealQueue.Enqueue(position);
            }

            while (revealQueue.TryDequeue(out int2 position))
            {
                if (!gridLookup.TryGetEntity(position.x, position.y, out Entity entity))
                    continue;

                CellState cell = entityManager.GetComponentData<CellState>(entity);
                if (CellFlags.Has(cell.Flags, CellFlags.Open) || CellFlags.Has(cell.Flags, CellFlags.Flag))
                    continue;

                cell.Flags |= CellFlags.Open;
                entityManager.SetComponentData(entity, cell);
                SystemAPI.SetComponentEnabled<VisualDirty>(entity, true);

                if (CellFlags.Has(cell.Flags, CellFlags.Mine))
                {
                    session.Status = GameStatus.Lost;
                    entityManager.SetComponentData(sessionEntity, session);
                    MarkAllMinesDirty(ref state, gridLookup);
                    revealQueue.Dispose();
                    commands.Dispose();
                    return;
                }

                session.OpenedSafeCells++;

                if (!session.TimerStarted)
                    session.TimerStarted = true;

                GameWinEvaluator.TrySetWon(ref session, entityManager, sessionEntity, gridLookup);

                if (cell.AdjacentMineCount == 0)
                    EnqueueNeighbors(revealQueue, gridLookup, position);
            }

            entityManager.SetComponentData(sessionEntity, session);
            revealQueue.Dispose();
            commands.Dispose();
        }

        private static void EnqueueNeighbors(NativeQueue<int2> queue, IGridLookup gridLookup, int2 position)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    int neighborX = position.x + dx;
                    int neighborY = position.y + dy;

                    if (gridLookup.TryGetEntity(neighborX, neighborY, out _))
                        queue.Enqueue(new int2(neighborX, neighborY));
                }
            }
        }

        private void MarkAllMinesDirty(ref SystemState state, IGridLookup gridLookup)
        {
            for (int x = 0; x < gridLookup.Width; x++)
            {
                for (int y = 0; y < gridLookup.Height; y++)
                {
                    Entity entity = gridLookup.GetEntity(x, y);
                    CellState cell = SystemAPI.GetComponent<CellState>(entity);

                    if (!CellFlags.Has(cell.Flags, CellFlags.Mine))
                        continue;

                    SystemAPI.SetComponentEnabled<VisualDirty>(entity, true);
                }
            }
        }
    }
}
