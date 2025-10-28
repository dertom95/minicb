using Data;
using Unity.Entities;

namespace Manager {
    public interface IAssetManager : IManager {
        /// <summary>
        ///  Get entityPrefab for buildingType
        /// </summary>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        Entity GetBuildingEntityPrefab(BuildingType buildingType);
        
        /// <summary>
        /// Get entityPrefab for resourcePrefabType
        /// </summary>
        /// <param name="resPrefType"></param>
        /// <returns></returns>
        Entity GetResourceEntityPrefab(ResourcePrefabType resPrefType);
        
        /// <summary>
        /// Register entity prefab for specific buildingType
        /// </summary>
        /// <param name="buildingType"></param>
        /// <param name="entityPrefab"></param>
        void RegisterBuildingEntityPrefab(BuildingType buildingType, Entity entityPrefab);

        /// <summary>
        /// Register entity prefab for specific resourcePrefabType
        /// </summary>
        /// <param name="resourcePrefType"></param>
        /// <param name="entityPrefab"></param>
        void RegisterResourceEntityPrefab(ResourcePrefabType resourcePrefType, Entity entityPrefab);
    }
}