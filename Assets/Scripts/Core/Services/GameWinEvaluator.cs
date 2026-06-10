using Minesweeper.Core.Components;
using Unity.Entities;

namespace Minesweeper.Core.Services
{
    public static class GameWinEvaluator
    {
        public static bool TrySetWon(
            ref GameSessionState session,
            EntityManager entityManager,
            Entity sessionEntity,
            IGridLookup gridLookup)
        {
            if (session.OpenedSafeCells != session.TotalSafeCells || session.FlagCountRemaining != 0)
                return false;

            if (!AreAllMinesFlagged(entityManager, gridLookup))
                return false;

            session.Status = GameStatus.Won;
            entityManager.SetComponentData(sessionEntity, session);
            return true;
        }

        public static bool AreAllMinesFlagged(EntityManager entityManager, IGridLookup gridLookup)
        {
            for (int x = 0; x < gridLookup.Width; x++)
            {
                for (int y = 0; y < gridLookup.Height; y++)
                {
                    Entity entity = gridLookup.GetEntity(x, y);
                    CellState cell = entityManager.GetComponentData<CellState>(entity);

                    if (CellFlags.Has(cell.Flags, CellFlags.Mine) && !CellFlags.Has(cell.Flags, CellFlags.Flag))
                        return false;
                }
            }

            return true;
        }
    }
}
