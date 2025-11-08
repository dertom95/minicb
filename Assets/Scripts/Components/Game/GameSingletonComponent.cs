using Unity.Entities;

namespace Components {
    public struct GameSingletonComponent : IComponentData {
        public Entity playerEntity;
    }
}