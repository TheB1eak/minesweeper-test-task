using Minesweeper.Bootstrap;
using Minesweeper.Core.Config;
using Minesweeper.Core.Services;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Minesweeper.Presentation
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameSettings _settings;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private GameHudView _gameHudView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_settings);
            builder.RegisterInstance(new EcsServices());
            builder.Register<GridLookup>(Lifetime.Singleton).As<IGridLookup>();
            builder.Register<CellViewRegistry>(Lifetime.Singleton).As<ICellViewRegistry>();
            builder.Register<GameWorldAccess>(Lifetime.Singleton).As<IGameWorldAccess>();
            builder.Register<GameSessionController>(Lifetime.Singleton).As<IGameSessionController>();
            builder.RegisterEntryPoint<GameInputHandler>(Lifetime.Singleton);

            if (_mainMenuView != null)
                builder.RegisterComponent(_mainMenuView);

            if (_gameHudView != null)
                builder.RegisterComponent(_gameHudView);

            if (_statusText != null)
            {
                builder.RegisterInstance(_statusText);
                builder.RegisterEntryPoint<GameUiPresenter>(Lifetime.Singleton);
            }
        }
    }
}
