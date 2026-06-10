using UnityEngine;

namespace Minesweeper.Core.Services
{
    public sealed class CellViewRegistry : ICellViewRegistry
    {
        private SpriteRenderer[,] _renderers;
        private int _width;
        private int _height;

        public void Reset(int width, int height)
        {
            _width = width;
            _height = height;
            _renderers = new SpriteRenderer[width, height];
        }

        public void Register(int x, int y, SpriteRenderer renderer)
        {
            _renderers[x, y] = renderer;
        }

        public bool TryGetRenderer(int x, int y, out SpriteRenderer renderer)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                renderer = null;
                return false;
            }

            renderer = _renderers[x, y];
            return renderer != null;
        }

        public void Clear()
        {
            if (_renderers == null)
                return;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_renderers[x, y] != null)
                        Object.Destroy(_renderers[x, y].gameObject);
                }
            }

            _renderers = null;
            _width = 0;
            _height = 0;
        }
    }
}
