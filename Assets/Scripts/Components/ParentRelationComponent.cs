using Unity.Entities;

namespace Components {
    public struct ParentRelationComponent : IComponentData {
        public Entity parentEntity;
    }
}

