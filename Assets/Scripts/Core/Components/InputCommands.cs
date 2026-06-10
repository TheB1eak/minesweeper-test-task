using Unity.Entities;
using Unity.Mathematics;

namespace Minesweeper.Core.Components
{
    public struct InputCommandsTag : IComponentData { }

    public struct RevealCellCommand : IBufferElementData
    {
        public int2 Position;
    }

    public struct FlagCellCommand : IBufferElementData
    {
        public int2 Position;
    }
}
