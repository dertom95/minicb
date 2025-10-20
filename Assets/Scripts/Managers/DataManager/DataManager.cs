using Components;
using Data;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace Manager {

    /// <summary>
    /// Manages data that does not fit in the ecs or cannot be stored there.
    /// This manager uses ushort(16bit opposed to 32bit for int) as key-type which restricts the amount of objects to be stored to 65535.
    /// 
    /// Notice: I use this as (artifical!) example that there are more integer-variant that don't need as much memory as int(4bytes). Especially
    ///         in DataOriented-Scenarios that can have an impact for CPU-Data-Caching. Keeping components (that do use the key) as compact as possible
    ///         can benefit the performance.
    /// </summary>
    public class DataManager : IManager {
        private ushort currentId;
        private Dictionary<ushort, object> dataStore;
        private Dictionary<BuildingType, Entity> buildingEntityPrefabs;
        private Dictionary<ResourceType, Entity> resourceEntityPrefabs;

        private static DataManager instance = new DataManager();

        /// <summary>
        /// Singleton Access
        /// </summary>
        public static DataManager Instance => instance;

        private DataManager() {
        }

        public void Init() {
            currentId = 1000;
            dataStore = new Dictionary<ushort, object>();
            buildingEntityPrefabs = new Dictionary<BuildingType, Entity>();
            resourceEntityPrefabs = new Dictionary<ResourceType, Entity>();
        }

        public void Dispose() {
        }


        /// <summary>
        /// Stores Data in the data store and returns a lookup-handle
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int StoreData(object data) {
            Assert.IsFalse(data == null,"It is not allowed to put null data into the store");
            Assert.IsFalse(currentId < short.MaxValue, "the datastore reached its limit! consider using int as key instead!");
            ushort objectId = currentId;
            dataStore[objectId] = data;
            currentId++;
            return objectId;
        }

        /// <summary>
        /// Remove data from the dataStore by objectId
        /// </summary>
        /// <param name="objectId"></param>
        public void RemoveData(ushort objectId) {
            Assert.IsTrue(dataStore.ContainsKey(objectId));
            dataStore.Remove(objectId);
        }

        /// <summary>
        /// Returns data from the dataStore
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public object GetData(ushort objectId) {
            Assert.IsTrue(dataStore.ContainsKey(objectId));
            return dataStore[objectId];
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
        /// Get resource entity in radius
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public Entity GetResourceEntityInRadius(float3 position, float radius, ResourceType resourceType, bool decreaseIterations = false) {
            Entity entity = PhysicsUtils.FindFirstEntityInRadius(
                World.DefaultGameObjectInjectionWorld, 
                position, 
                radius, 
                Config.LAYER_RESOURCE, 
                (em, entity) => {
                    if (em.HasComponent<ResourceComponent>(entity)) {
                        ResourceComponent resourceComponent = em.GetComponentData<ResourceComponent>(entity);
                        if (resourceComponent.resourceType == resourceType && resourceComponent.iterationsLeft > 0) {
                            if (decreaseIterations) {
                                resourceComponent.iterationsLeft--;
                                resourceComponent.pendingJobs++;
                                em.SetComponentData(entity, resourceComponent);
                            }
                            return true;
                        }
                    }
                    return false;
                }
            );
            return entity;
        }


    }

}