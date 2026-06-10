using Unity.Entities;

namespace Minesweeper.Core.Components
{
    public struct CellState : IComponentData
    {
        public byte Flags;
        public byte AdjacentMineCount;
    }

    public static class CellFlags
    {
        public const byte Open = 1;
        public const byte Flag = 2;
        public const byte Mine = 4;

        public static bool Has(byte flags, byte flag) => (flags & flag) != 0;
    }

    public struct GridPosition : IComponentData
    {
        public int X;
        public int Y;
    }

    public struct CellTag : IComponentData { }

    public struct VisualDirty : IComponentData, IEnableableComponent { }
}
