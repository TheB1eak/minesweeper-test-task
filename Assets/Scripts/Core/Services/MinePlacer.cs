using Minesweeper.Core.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Minesweeper.Core.Services
{
    public static class MinePlacer
    {
        public static void PlaceMines(
            EntityManager entityManager,
            IGridLookup gridLookup,
            int mineCount,
            uint seed,
            int safeX = -1,
            int safeY = -1)
        {
            Random random = new Random(seed == 0 ? 1u : seed);
            int remainingMines = mineCount;

            while (remainingMines > 0)
            {
                int x = random.NextInt(0, gridLookup.Width);
                int y = random.NextInt(0, gridLookup.Height);

                if (safeX >= 0 && math.abs(x - safeX) <= 1 && math.abs(y - safeY) <= 1)
                    continue;

                Entity entity = gridLookup.GetEntity(x, y);
                CellState cell = entityManager.GetComponentData<CellState>(entity);

                if (CellFlags.Has(cell.Flags, CellFlags.Mine))
                    continue;

                cell.Flags |= CellFlags.Mine;
                entityManager.SetComponentData(entity, cell);
                remainingMines--;
            }
        }
    }
}
