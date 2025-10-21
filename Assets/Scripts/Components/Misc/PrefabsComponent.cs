using Data;
using Unity.Entities;

namespace Components {
    /// <summary>
    /// Single element to buffer building-entity-prefabs
    /// </summary>
    public struct BuildingPrefabBufferElement : IBufferElementData {
        public BuildingType type;
        public Entity prefabEntity;
    }

    /// <summary>
    /// Single element to buffer resource-entity-prefabs
    /// </summary>
    public struct ResourcePrefabBufferElement : IBufferElementData {
        public ResourceType type;
        public Entity prefabEntity;
    }
}
