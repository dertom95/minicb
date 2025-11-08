using Unity.Entities;

public struct HouseComponent : IComponentData {
    /// <summary>
    /// Increase the maxAmount of global settlers 
    /// </summary>
    public byte increaseSettlerAmount;
}