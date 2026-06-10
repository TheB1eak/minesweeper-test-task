using Minesweeper.Bootstrap;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Minesweeper.Presentation
{
    public sealed class MainMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject _menuRoot;
        [SerializeField] private GameObject _hudRoot;
        [SerializeField] private Button _startButton;

        private IGameSessionController _sessionController;

        [Inject]
        public void Construct(IGameSessionController sessionController)
        {
            _sessionController = sessionController;
        }

        private void Start()
        {
            UiBindings.SetPanelActive(_hudRoot, false);
            UiBindings.SetPanelActive(_menuRoot, true);
            UiBindings.BindButton(_startButton, StartGame);
        }

        private void OnDestroy()
        {
            UiBindings.UnbindButton(_startButton, StartGame);
        }

        public void ReturnToMainMenu()
        {
            _sessionController.Teardown();
            UiBindings.SetPanelActive(_hudRoot, false);
            UiBindings.SetPanelActive(_menuRoot, true);
        }

        private void StartGame()
        {
            _sessionController.Initialize();
            UiBindings.SetPanelActive(_menuRoot, false);
            UiBindings.SetPanelActive(_hudRoot, true);
        }
    }
}
