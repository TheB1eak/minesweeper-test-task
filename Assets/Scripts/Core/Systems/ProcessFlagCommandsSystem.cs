using Minesweeper.Core.Components;
using Minesweeper.Core.Services;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Minesweeper.Core.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProcessRevealCommandsSystem))]
    public partial struct ProcessFlagCommandsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonEntity<GameSessionTag>(out Entity sessionEntity))
                return;

            GameSessionState session = SystemAPI.GetComponent<GameSessionState>(sessionEntity);
            if (session.Status != GameStatus.Playing)
                return;

            if (!SystemAPI.TryGetSingletonBuffer<FlagCellCommand>(out DynamicBuffer<FlagCellCommand> flagCommands))
                return;

            if (flagCommands.Length == 0)
                return;

            EcsServices services = SystemAPI.ManagedAPI.GetSingleton<EcsServices>();
            IGridLookup gridLookup = services.GridLookup;
            EntityManager entityManager = state.EntityManager;

            NativeArray<FlagCellCommand> commands =
                new NativeArray<FlagCellCommand>(flagCommands.Length, Allocator.Temp);

            for (int i = 0; i < flagCommands.Length; i++)
                commands[i] = flagCommands[i];

            flagCommands.Clear();

            for (int i = 0; i < commands.Length; i++)
            {
                int2 position = commands[i].Position;
                if (!gridLookup.TryGetEntity(position.x, position.y, out Entity entity))
                    continue;

                CellState cell = entityManager.GetComponentData<CellState>(entity);
                if (CellFlags.Has(cell.Flags, CellFlags.Open))
                    continue;

                if (CellFlags.Has(cell.Flags, CellFlags.Flag))
                {
                    cell.Flags &= unchecked((byte)~CellFlags.Flag);
                    session.FlagCountRemaining = math.min(session.MineCount, session.FlagCountRemaining + 1);
                }
                else
                {
                    cell.Flags |= CellFlags.Flag;
                    session.FlagCountRemaining = math.max(0, session.FlagCountRemaining - 1);
                }

                entityManager.SetComponentData(entity, cell);
                SystemAPI.SetComponentEnabled<VisualDirty>(entity, true);
                GameWinEvaluator.TrySetWon(ref session, entityManager, sessionEntity, gridLookup);
            }

            entityManager.SetComponentData(sessionEntity, session);
            commands.Dispose();
        }
    }
}
