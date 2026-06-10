using Unity.Entities;

namespace Minesweeper.Core.Services
{
    public interface IGameWorldAccess
    {
        World World { get; }
        Entity SessionEntity { get; }
        Entity InputEntity { get; }
        Entity ServicesEntity { get; }
        bool IsInitialized { get; }
        void SetEntities(Entity sessionEntity, Entity inputEntity, Entity servicesEntity);
        void ClearEntities();
    }
}
