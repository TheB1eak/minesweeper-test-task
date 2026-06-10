using Minesweeper.Bootstrap;
using Minesweeper.Core.Components;
using Minesweeper.Core.Services;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Minesweeper.Presentation
{
    public sealed class GameHudView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private Button _gameOverRestartButton;
        [SerializeField] private Button _gameOverMainMenuButton;

        private IGameSessionController _sessionController;
        private IGameWorldAccess _worldAccess;
        private MainMenuView _mainMenuView;
        private bool _gameOverPanelVisible;

        [Inject]
        public void Construct(
            IGameSessionController sessionController,
            IGameWorldAccess worldAccess,
            MainMenuView mainMenuView)
        {
            _sessionController = sessionController;
            _worldAccess = worldAccess;
            _mainMenuView = mainMenuView;
        }

        private void Start()
        {
            UiBindings.SetPanelActive(_pausePanel, false);
            UiBindings.SetPanelActive(_gameOverPanel, false);

            UiBindings.BindButton(_pauseButton, OpenPauseMenu);
            UiBindings.BindButton(_continueButton, ContinueGame);
            UiBindings.BindButton(_restartButton, RestartGame);
            UiBindings.BindButton(_mainMenuButton, ExitToMainMenu);
            UiBindings.BindButton(_gameOverRestartButton, RestartGame);
            UiBindings.BindButton(_gameOverMainMenuButton, ExitToMainMenu);

            UpdateTimerText(0f, false);
        }

        private void Update()
        {
            if (!TryGetSession(out GameSessionState session))
            {
                UpdateTimerText(0f, false);
                HideGameOverPanel();
                return;
            }

            UpdateTimerText(session.ElapsedTime, session.TimerStarted);
            UpdateGameOverPanel(session);
        }

        private void OnDestroy()
        {
            UiBindings.UnbindButton(_pauseButton, OpenPauseMenu);
            UiBindings.UnbindButton(_continueButton, ContinueGame);
            UiBindings.UnbindButton(_restartButton, RestartGame);
            UiBindings.UnbindButton(_mainMenuButton, ExitToMainMenu);
            UiBindings.UnbindButton(_gameOverRestartButton, RestartGame);
            UiBindings.UnbindButton(_gameOverMainMenuButton, ExitToMainMenu);
        }

        private void OpenPauseMenu()
        {
            if (!TryGetSession(out GameSessionState session))
                return;

            if (session.Status != GameStatus.Playing)
                return;

            session.Status = GameStatus.Paused;
            SetSession(session);
            UiBindings.SetPanelActive(_pausePanel, true);
        }

        private void ContinueGame()
        {
            if (!TryGetSession(out GameSessionState session))
                return;

            if (session.Status != GameStatus.Paused)
                return;

            session.Status = GameStatus.Playing;
            SetSession(session);
            UiBindings.SetPanelActive(_pausePanel, false);
        }

        private void RestartGame()
        {
            UiBindings.SetPanelActive(_pausePanel, false);
            HideGameOverPanel();
            _sessionController.Restart();
        }

        private void ExitToMainMenu()
        {
            UiBindings.SetPanelActive(_pausePanel, false);
            HideGameOverPanel();
            _mainMenuView.ReturnToMainMenu();
        }

        private bool TryGetSession(out GameSessionState session)
        {
            session = default;

            if (_worldAccess == null || !_worldAccess.IsInitialized)
                return false;

            EntityManager entityManager = _worldAccess.World.EntityManager;
            session = entityManager.GetComponentData<GameSessionState>(_worldAccess.SessionEntity);
            return true;
        }

        private void SetSession(GameSessionState session)
        {
            EntityManager entityManager = _worldAccess.World.EntityManager;
            entityManager.SetComponentData(_worldAccess.SessionEntity, session);
        }

        private void UpdateGameOverPanel(GameSessionState session)
        {
            if (session.Status == GameStatus.Won || session.Status == GameStatus.Lost)
            {
                if (!_gameOverPanelVisible)
                {
                    if (_gameOverText != null)
                    {
                        _gameOverText.text = session.Status == GameStatus.Won
                            ? "You won!\nGame over."
                            : "Game over.";
                    }

                    UiBindings.SetPanelActive(_pausePanel, false);
                    UiBindings.SetPanelActive(_gameOverPanel, true);
                    _gameOverPanelVisible = true;
                }

                return;
            }

            HideGameOverPanel();
        }

        private void HideGameOverPanel()
        {
            if (!_gameOverPanelVisible)
                return;

            UiBindings.SetPanelActive(_gameOverPanel, false);
            _gameOverPanelVisible = false;
        }

        private void UpdateTimerText(float elapsedSeconds, bool timerStarted)
        {
            if (_timerText == null)
                return;

            _timerText.text = timerStarted ? FormatTime(elapsedSeconds) : "00:00";
        }

        private static string FormatTime(float elapsedSeconds)
        {
            int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(elapsedSeconds));
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }
    }
}
