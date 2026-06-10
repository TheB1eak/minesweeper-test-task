using Unity.Entities;

namespace Minesweeper.Core.Services
{
    public sealed class GameWorldAccess : IGameWorldAccess
    {
        public World World => World.DefaultGameObjectInjectionWorld;
        public Entity SessionEntity { get; private set; }
        public Entity InputEntity { get; private set; }
        public Entity ServicesEntity { get; private set; }
        public bool IsInitialized => SessionEntity != Entity.Null;

        public void SetEntities(Entity sessionEntity, Entity inputEntity, Entity servicesEntity)
        {
            SessionEntity = sessionEntity;
            InputEntity = inputEntity;
            ServicesEntity = servicesEntity;
        }

        public void ClearEntities()
        {
            SessionEntity = Entity.Null;
            InputEntity = Entity.Null;
            ServicesEntity = Entity.Null;
        }
    }
}
