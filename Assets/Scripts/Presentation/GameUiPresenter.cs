using Minesweeper.Core.Components;
using Minesweeper.Core.Services;
using TMPro;
using Unity.Entities;
using VContainer.Unity;

namespace Minesweeper.Presentation
{
    public sealed class GameUiPresenter : ITickable
    {
        private readonly IGameWorldAccess _worldAccess;
        private readonly TextMeshProUGUI _statusText;

        public GameUiPresenter(IGameWorldAccess worldAccess, TextMeshProUGUI statusText)
        {
            _worldAccess = worldAccess;
            _statusText = statusText;
        }

        public void Tick()
        {
            if (_statusText == null || !_worldAccess.IsInitialized)
                return;

            EntityManager entityManager = _worldAccess.World.EntityManager;
            GameSessionState session =
                entityManager.GetComponentData<GameSessionState>(_worldAccess.SessionEntity);

            _statusText.text = session.Status switch
            {
                GameStatus.Won => "Game over",
                GameStatus.Lost => "Game over",
                GameStatus.Paused => "Paused",
                _ => $"Flags: {session.FlagCountRemaining} | Opened: {session.OpenedSafeCells}/{session.TotalSafeCells}"
            };
        }
    }
}
