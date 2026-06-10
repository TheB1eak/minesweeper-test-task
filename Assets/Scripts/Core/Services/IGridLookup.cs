using Unity.Entities;

namespace Minesweeper.Core.Services
{
    public interface IGridLookup
    {
        int Width { get; }
        int Height { get; }
        void Reset(int width, int height);
        void Register(int x, int y, Entity entity);
        bool TryGetEntity(int x, int y, out Entity entity);
        Entity GetEntity(int x, int y);
    }
}
