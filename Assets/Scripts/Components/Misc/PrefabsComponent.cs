using Data;
using Unity.Collections;
using Unity.Entities;

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
    public ResourcePrefabType type;
    public Entity prefabEntity;
}

public struct SettlerPrefabBufferElement : IBufferElementData {
    public Entity prefabEntity;
}
