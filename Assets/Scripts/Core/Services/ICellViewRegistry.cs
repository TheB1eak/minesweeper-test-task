using UnityEngine;

namespace Minesweeper.Core.Services
{
    public interface ICellViewRegistry
    {
        void Reset(int width, int height);
        void Register(int x, int y, SpriteRenderer renderer);
        bool TryGetRenderer(int x, int y, out SpriteRenderer renderer);
        void Clear();
    }
}
