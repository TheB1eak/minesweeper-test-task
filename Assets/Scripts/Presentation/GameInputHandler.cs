using Minesweeper.Core.Components;
using Minesweeper.Core.Services;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using VContainer.Unity;

namespace Minesweeper.Presentation
{
    public sealed class GameInputHandler : ITickable
    {
        private readonly IGameWorldAccess _worldAccess;

        public GameInputHandler(IGameWorldAccess worldAccess)
        {
            _worldAccess = worldAccess;
        }

        public void Tick()
        {
            if (!TryGetPlayingSession(out EntityManager entityManager, out Entity inputEntity))
                return;

            DynamicBuffer<RevealCellCommand> revealCommands =
                entityManager.GetBuffer<RevealCellCommand>(inputEntity);

            DynamicBuffer<FlagCellCommand> flagCommands =
                entityManager.GetBuffer<FlagCellCommand>(inputEntity);

            int2 clickedPosition = GetClickedCellPosition();

            if (Input.GetMouseButtonDown(0))
                revealCommands.Add(new RevealCellCommand { Position = clickedPosition });

            if (Input.GetMouseButtonDown(1))
                flagCommands.Add(new FlagCellCommand { Position = clickedPosition });
        }

        private bool TryGetPlayingSession(out EntityManager entityManager, out Entity inputEntity)
        {
            entityManager = default;
            inputEntity = Entity.Null;

            if (_worldAccess == null || !_worldAccess.IsInitialized)
                return false;

            entityManager = _worldAccess.World.EntityManager;
            inputEntity = _worldAccess.InputEntity;

            GameSessionState session =
                entityManager.GetComponentData<GameSessionState>(_worldAccess.SessionEntity);

            return session.Status == GameStatus.Playing;
        }

        private static int2 GetClickedCellPosition()
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new int2(
                Mathf.RoundToInt(worldPosition.x),
                Mathf.RoundToInt(worldPosition.y));
        }
    }
}
