using Unity.Entities;

namespace Minesweeper.Core.Services
{
    public sealed class GridLookup : IGridLookup
    {
        private Entity[,] _entities;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public void Reset(int width, int height)
        {
            Width = width;
            Height = height;
            _entities = new Entity[width, height];
        }

        public void Register(int x, int y, Entity entity)
        {
            _entities[x, y] = entity;
        }

        public bool TryGetEntity(int x, int y, out Entity entity)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                entity = Entity.Null;
                return false;
            }

            entity = _entities[x, y];
            return entity != Entity.Null;
        }

        public Entity GetEntity(int x, int y) => _entities[x, y];
    }
}
