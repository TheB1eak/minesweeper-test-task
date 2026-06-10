using Minesweeper.Core.Config;
using Unity.Entities;

namespace Minesweeper.Core.Services
{
    public sealed class EcsServices : IComponentData
    {
        public IGridLookup GridLookup;
        public ICellViewRegistry CellViewRegistry;
        public GameSettings Settings;
    }
}
