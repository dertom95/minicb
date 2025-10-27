using Data;
using Unity.Entities;

namespace Manager {
    public interface IAssetManager : IManager {
        Entity GetBuildingEntityPrefab(BuildingType buildingType);
        Entity GetResourceEntityPrefab(ResourcePrefabType resPrefType);
        void RegisterBuildingEntityPrefab(BuildingType buildingType, Entity entityPrefab);
        void RegisterResourceEntityPrefab(ResourcePrefabType resourcePrefType, Entity entityPrefab);
    }
}