using Minesweeper.Core.Components;
using Minesweeper.Core.Config;
using Minesweeper.Core.Services;
using Unity.Entities;
using UnityEngine;

namespace Minesweeper.Core.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProcessFlagCommandsSystem))]
    public partial struct CellVisualSyncSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonEntity<GameSessionTag>(out Entity sessionEntity))
                return;

            GameSessionState session = SystemAPI.GetComponent<GameSessionState>(sessionEntity);
            EcsServices services = SystemAPI.ManagedAPI.GetSingleton<EcsServices>();
            GameSettings settings = services.Settings;
            ICellViewRegistry viewRegistry = services.CellViewRegistry;

            foreach (var (cellState, gridPosition, entity) in SystemAPI
                         .Query<RefRO<CellState>, RefRO<GridPosition>>()
                         .WithAll<VisualDirty>()
                         .WithEntityAccess())
            {
                int x = gridPosition.ValueRO.X;
                int y = gridPosition.ValueRO.Y;

                if (!viewRegistry.TryGetRenderer(x, y, out SpriteRenderer renderer))
                    continue;

                renderer.sprite = ResolveSprite(cellState.ValueRO, session.Status, settings);
                SystemAPI.SetComponentEnabled<VisualDirty>(entity, false);
            }
        }

        private static Sprite ResolveSprite(CellState cell, GameStatus status, GameSettings settings)
        {
            if (status == GameStatus.Lost && CellFlags.Has(cell.Flags, CellFlags.Mine))
                return settings.MineCellSprite;

            if (CellFlags.Has(cell.Flags, CellFlags.Flag))
                return settings.FlagCellSprite;

            if (!CellFlags.Has(cell.Flags, CellFlags.Open))
                return settings.ClosedCellSprite;

            if (cell.AdjacentMineCount > 0)
                return settings.NumberSprites[cell.AdjacentMineCount - 1];

            return settings.EmptyCellSprite;
        }
    }
}
