using Data;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.Assertions;

namespace Manager {
    public class AssetManager : IManager {
        private static AssetManager instance = new AssetManager();
        public static AssetManager Instance => instance;
        
        private AssetManager() { }

        /// <summary>
        /// Lookup BuildingType->EntityPrefab
        /// </summary>
        private Dictionary<BuildingType, Entity> buildingEntityPrefabs;

        /// <summary>
        /// Lookup ResourceType->EntityPrefab
        /// </summary>
        private Dictionary<ResourcePrefabType, Entity> resourceEntityPrefabs;

        EntityManager entityManager;

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            buildingEntityPrefabs = new Dictionary<BuildingType, Entity>();
            resourceEntityPrefabs = new Dictionary<ResourcePrefabType, Entity>();
        }

        /// <summary>
        /// Register Building EntityPrefab (only one per BuildingType)
        /// </summary>
        /// <param name="buildingType"></param>
        /// <param name="entityPrefab"></param>
        public void RegisterBuildingEntityPrefab(BuildingType buildingType, Entity entityPrefab) {
            Assert.IsFalse(buildingEntityPrefabs.ContainsKey(buildingType));

            buildingEntityPrefabs[buildingType] = entityPrefab;
        }

        /// <summary>
        /// Returns EntityPrefab to specific buildingsType
        /// </summary>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        public Entity GetBuildingEntityPrefab(BuildingType buildingType) {
            Assert.IsTrue(buildingEntityPrefabs.ContainsKey(buildingType));

            return buildingEntityPrefabs[buildingType];
        }

        /// <summary>
        /// Register Building EntityPrefab (only one per BuildingType)
        /// </summary>
        /// <param name="resourcePrefType"></param>
        /// <param name="entityPrefab"></param>
        public void RegisterResourceEntityPrefab(ResourcePrefabType resourcePrefType, Entity entityPrefab) {
            Assert.IsFalse(resourceEntityPrefabs.ContainsKey(resourcePrefType));

            resourceEntityPrefabs[resourcePrefType] = entityPrefab;
        }

        /// <summary>
        /// Returns EntityPrefab to specific buildingsType
        /// </summary>
        /// <param name="resPrefType"></param>
        /// <returns></returns>
        public Entity GetResourceEntityPrefab(ResourcePrefabType resPrefType) {
            Assert.IsTrue(resourceEntityPrefabs.ContainsKey(resPrefType));

            return resourceEntityPrefabs[resPrefType];
        }


        public void Dispose() {
        }

    }
}


