using Minesweeper.Core.Components;
using Unity.Entities;

namespace Minesweeper.Core.Services
{
    public static class AdjacentMineCalculator
    {
        public static void Calculate(EntityManager entityManager, IGridLookup gridLookup)
        {
            for (int x = 0; x < gridLookup.Width; x++)
            {
                for (int y = 0; y < gridLookup.Height; y++)
                {
                    Entity entity = gridLookup.GetEntity(x, y);
                    CellState cell = entityManager.GetComponentData<CellState>(entity);
                    cell.AdjacentMineCount = (byte)CountAdjacentMines(entityManager, gridLookup, x, y);
                    entityManager.SetComponentData(entity, cell);
                }
            }
        }

        private static int CountAdjacentMines(EntityManager entityManager, IGridLookup gridLookup, int x, int y)
        {
            int count = 0;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    if (!gridLookup.TryGetEntity(x + dx, y + dy, out Entity neighbor))
                        continue;

                    CellState neighborCell = entityManager.GetComponentData<CellState>(neighbor);
                    if (CellFlags.Has(neighborCell.Flags, CellFlags.Mine))
                        count++;
                }
            }

            return count;
        }
    }
}
