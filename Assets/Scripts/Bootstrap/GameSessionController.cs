using Minesweeper.Core.Components;
using Minesweeper.Core.Config;
using Minesweeper.Core.Services;
using Unity.Entities;
using UnityEngine;

namespace Minesweeper.Bootstrap
{
    public sealed class GameSessionController : IGameSessionController
    {
        private readonly GameSettings _settings;
        private readonly IGridLookup _gridLookup;
        private readonly ICellViewRegistry _cellViewRegistry;
        private readonly IGameWorldAccess _worldAccess;
        private readonly EcsServices _ecsServices;

        public GameSessionController(
            GameSettings settings,
            IGridLookup gridLookup,
            ICellViewRegistry cellViewRegistry,
            IGameWorldAccess worldAccess,
            EcsServices ecsServices)
        {
            _settings = settings;
            _gridLookup = gridLookup;
            _cellViewRegistry = cellViewRegistry;
            _worldAccess = worldAccess;
            _ecsServices = ecsServices;
        }

        public void Initialize()
        {
            if (_worldAccess.IsInitialized)
                Teardown();

            EntityManager entityManager = _worldAccess.World.EntityManager;
            int width = _settings.Width;
            int height = _settings.Height;
            int mineCount = _settings.ResolveMineCount();

            _gridLookup.Reset(width, height);
            _cellViewRegistry.Reset(width, height);

            Entity servicesEntity = CreateServicesEntity(entityManager);
            Entity sessionEntity = CreateSessionEntity(entityManager, width, height, mineCount);
            Entity inputEntity = CreateInputEntity(entityManager);

            CreateCells(entityManager, width, height);

            if (!_settings.SafeFirstClick)
                PlaceMinesAtStart(entityManager, sessionEntity, mineCount);

            _worldAccess.SetEntities(sessionEntity, inputEntity, servicesEntity);
        }

        public void Teardown()
        {
            if (!_worldAccess.IsInitialized)
                return;

            EntityManager entityManager = _worldAccess.World.EntityManager;
            EntityQuery cellQuery = entityManager.CreateEntityQuery(typeof(CellTag));

            entityManager.DestroyEntity(cellQuery);

            if (entityManager.Exists(_worldAccess.SessionEntity))
                entityManager.DestroyEntity(_worldAccess.SessionEntity);

            if (entityManager.Exists(_worldAccess.InputEntity))
                entityManager.DestroyEntity(_worldAccess.InputEntity);

            if (entityManager.Exists(_worldAccess.ServicesEntity))
                entityManager.DestroyEntity(_worldAccess.ServicesEntity);

            _cellViewRegistry.Clear();
            _worldAccess.ClearEntities();
        }

        public void Restart()
        {
            Initialize();
        }

        private void PlaceMinesAtStart(EntityManager entityManager, Entity sessionEntity, int mineCount)
        {
            MinePlacer.PlaceMines(
                entityManager,
                _gridLookup,
                mineCount,
                (uint)Random.Range(1, int.MaxValue));

            AdjacentMineCalculator.Calculate(entityManager, _gridLookup);

            GameSessionState session = entityManager.GetComponentData<GameSessionState>(sessionEntity);
            session.MinesPlaced = true;
            entityManager.SetComponentData(sessionEntity, session);
        }

        private Entity CreateServicesEntity(EntityManager entityManager)
        {
            _ecsServices.GridLookup = _gridLookup;
            _ecsServices.CellViewRegistry = _cellViewRegistry;
            _ecsServices.Settings = _settings;

            Entity entity = entityManager.CreateEntity(typeof(EcsServices));
            entityManager.AddComponentData(entity, _ecsServices);
            return entity;
        }

        private static Entity CreateSessionEntity(EntityManager entityManager, int width, int height, int mineCount)
        {
            Entity entity = entityManager.CreateEntity(typeof(GameSessionTag), typeof(GameSessionState));
            entityManager.SetComponentData(entity, new GameSessionState
            {
                Status = GameStatus.Playing,
                FlagCountRemaining = mineCount,
                OpenedSafeCells = 0,
                TotalSafeCells = width * height - mineCount,
                MineCount = mineCount,
                GridWidth = width,
                GridHeight = height
            });

            return entity;
        }

        private static Entity CreateInputEntity(EntityManager entityManager)
        {
            Entity entity = entityManager.CreateEntity(typeof(InputCommandsTag));
            entityManager.AddBuffer<RevealCellCommand>(entity);
            entityManager.AddBuffer<FlagCellCommand>(entity);
            return entity;
        }

        private void CreateCells(EntityManager entityManager, int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject cellObject = Object.Instantiate(
                        _settings.CellPrefab,
                        new Vector3(x, y, 0f),
                        Quaternion.identity);

                    SpriteRenderer renderer = cellObject.GetComponent<SpriteRenderer>();
                    renderer.sprite = _settings.ClosedCellSprite;

                    Entity entity = entityManager.CreateEntity(
                        typeof(CellTag),
                        typeof(CellState),
                        typeof(GridPosition),
                        typeof(VisualDirty));

                    entityManager.SetComponentData(entity, new CellState());
                    entityManager.SetComponentData(entity, new GridPosition { X = x, Y = y });
                    entityManager.SetComponentEnabled<VisualDirty>(entity, false);

                    _gridLookup.Register(x, y, entity);
                    _cellViewRegistry.Register(x, y, renderer);
                }
            }
        }
    }
}
