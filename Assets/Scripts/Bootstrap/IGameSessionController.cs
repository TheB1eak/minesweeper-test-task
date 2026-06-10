namespace Minesweeper.Bootstrap
{
    public interface IGameSessionController
    {
        void Initialize();
        void Teardown();
        void Restart();
    }
}
