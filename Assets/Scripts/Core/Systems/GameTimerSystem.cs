using Minesweeper.Core.Components;
using Unity.Entities;

namespace Minesweeper.Core.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct GameTimerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonEntity<GameSessionTag>(out Entity sessionEntity))
                return;

            GameSessionState session = SystemAPI.GetComponent<GameSessionState>(sessionEntity);

            if (session.Status != GameStatus.Playing || !session.TimerStarted)
                return;

            session.ElapsedTime += SystemAPI.Time.DeltaTime;
            SystemAPI.SetComponent(sessionEntity, session);
        }
    }
}
