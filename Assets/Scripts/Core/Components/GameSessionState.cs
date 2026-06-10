using Unity.Entities;

namespace Minesweeper.Core.Components
{
    public enum GameStatus : byte
    {
        Playing = 0,
        Paused = 1,
        Won = 2,
        Lost = 3
    }

    public struct GameSessionState : IComponentData
    {
        public GameStatus Status;
        public bool MinesPlaced;
        public bool TimerStarted;
        public float ElapsedTime;
        public int FlagCountRemaining;
        public int OpenedSafeCells;
        public int TotalSafeCells;
        public int MineCount;
        public int GridWidth;
        public int GridHeight;
    }

    public struct GameSessionTag : IComponentData { }
}
