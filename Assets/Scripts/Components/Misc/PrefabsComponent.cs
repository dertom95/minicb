using Data;
using Unity.Entities;

/// <summary>
/// Single element to buffer building-entity-prefabs
/// </summary>
public struct BuildingPrefabBufferElement : IBufferElementData {
    public BuildingType type;
    public Entity prefabEntity;
}