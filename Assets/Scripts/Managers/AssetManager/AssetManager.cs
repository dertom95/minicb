using Data;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.Assertions;

namespace Manager {
    public class AssetManager : IManager, IAssetManager {
        public AssetManager() { }

        /// <summary>
        /// Lookup BuildingType->EntityPrefab
        /// </summary>
        private Dictionary<BuildingType, Entity> buildingEntityPrefabs;

        /// <summary>
        /// Lookup ResourceType->EntityPrefab
        /// </summary>
        private Dictionary<ResourcePrefabType, Entity> resourceEntityPrefabs;

        /// <summary>
        /// Lookup ResourceType->EntityPrefab
        /// </summary>
        private Dictionary<SettlerType, Entity> settlerEntityPrefabs;

        EntityManager entityManager;

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            buildingEntityPrefabs = new Dictionary<BuildingType, Entity>();
            resourceEntityPrefabs = new Dictionary<ResourcePrefabType, Entity>();
            settlerEntityPrefabs = new Dictionary<SettlerType, Entity>();
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


        /// <summary>
        /// Register Settler EntityPrefab (only one per SettlerType)
        /// </summary>
        /// <param name="settlerType"></param>
        /// <param name="entityPrefab"></param>
        public void RegisterSettlerEntityPrefab(SettlerType settlerType, Entity entityPrefab) {
            Assert.IsFalse(settlerEntityPrefabs.ContainsKey(settlerType));

            settlerEntityPrefabs[settlerType] = entityPrefab;
        }

        /// <summary>
        /// Returns EntityPrefab to specific settlerType
        /// </summary>
        /// <param name="buildsettlerTypeingType"></param>
        /// <returns></returns>
        public Entity GetSettlerEntityPrefab(SettlerType settlerType) {
            Assert.IsTrue(settlerEntityPrefabs.ContainsKey(settlerType));

            return settlerEntityPrefabs[settlerType];
        }


        public void Dispose() {
        }

    }
}


