using Data;
using Unity.Entities;
using Unity.Mathematics;

namespace Manager {
    public interface IBuildingManager : IManager {
        Entity SpawnBuilding(BuildingType buildingType, float3 position);
        Entity SpawnBuilding(BuildingType buildingType, float3 position, float scale, float3 rotation, bool immediateBuild = false);
    }
}