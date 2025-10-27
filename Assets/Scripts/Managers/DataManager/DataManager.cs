using Components;
using Data;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Manager {

    /// <summary>
    /// Manages data that does not fit in the ecs or cannot be stored there.
    /// This manager uses ushort(16bit opposed to 32bit for int) as key-type which restricts the amount of objects to be stored to 65535.
    /// 
    /// Notice: I use this as (artifical!) example that there are more integer-variant that don't need as much memory as int(4bytes). Especially
    ///         in DataOriented-Scenarios that can have an impact for CPU-Data-Caching. Keeping components (that do use the key) as compact as possible
    ///         can benefit the performance.
    /// </summary>
    public class DataManager : IManager, IDataManager {
        private EntityManager entityManager;
        private IAssetManager assetManager;
        private IPhysicsManager physicsManager;

        /// <summary>
        /// next id used for datastore
        /// </summary>
        private ushort currentId;
        /// <summary>
        /// data store - dictionary
        /// </summary>
        private Dictionary<ushort, object> dataStore;

        // TODO: Extract Inventory in its own Manager

        /// <summary>
        /// Global Inventory - Store
        /// </summary>
        private Dictionary<ResourceType, int> globalInventory;
        /// <summary>
        /// Global Inventory - Limits to prevent settlers from taking Jobs that will result in overflowing the limit. (not 100% strict)
        /// </summary>
        private Dictionary<ResourceType, int> globalInventoryLimits;

        /// <summary>
        /// Event triggered if the inventory got changed
        /// </summary>
        private event EventHandler<Dictionary<ResourceType, int>> EventInventoryChanged;

        public DataManager() {
        }

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            assetManager = Mgr.assetManager;
            physicsManager = Mgr.physicsManager;

            currentId = 1000;
            dataStore = new Dictionary<ushort, object>();
            InitInventory();
        }

        private void InitInventory() {
            globalInventory = new Dictionary<ResourceType, int>();
            globalInventoryLimits = new Dictionary<ResourceType, int>();
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType))) {
                globalInventory[resourceType] = 0;
                globalInventoryLimits[resourceType] = 75;
            }
            foreach (ResourceAmount res in Config.Instance.InitialInventory) {
                globalInventory[res.resourceType] = res.resourceAmount;
            }
            TriggerInventoryChanged();
        }

        public void Dispose() {
        }


        /// <summary>
        /// Stores Data in the data store and returns a lookup-handle
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int StoreData(object data) {
            Assert.IsFalse(data == null, "It is not allowed to put null data into the store");
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
        /// Get resource entity in radius
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public Entity GetResourceEntityInRadius(float3 position, float radius, ResourceType resourceType) {
            Entity entity = physicsManager.FindFirstEntityInRadius(
                World.DefaultGameObjectInjectionWorld,
                position,
                radius,
                1u << Config.LAYER_RESOURCE,
                (em, entity) => {
                    if (em.HasComponent<ResourceComponent>(entity)) {
                        Assert.IsTrue(em.HasComponent<IterationsComponent>(entity), "Each resource component must have an IterationsComponent!");

                        ResourceComponent resourceComponent = em.GetComponentData<ResourceComponent>(entity);
                        if (resourceComponent.resourceType == resourceType) {
                            IterationsComponent iterationsComp = em.GetComponentData<IterationsComponent>(entity);
                            return iterationsComp.iterationsLeft > 0;
                        }
                    }
                    return false;
                }
            );
            return entity;
        }

        /// <summary>
        /// Add resource to inventory
        /// </summary>
        /// <param name="inventoryComponent"></param>
        /// <returns></returns>
        public int AddToGlobalInventory(ResourceType res, int amount) {
            int newAmount = globalInventory[res] + amount;
            globalInventory[res] = newAmount;
            UnityEngine.Debug.Log($"Added to globalInventory: {res}:{amount} => Total:{amount}");
            TriggerInventoryChanged();
            return newAmount;
        }

        /// <summary>
        /// Add resource to inventory
        /// </summary>
        /// <param name="resAmount"></param>
        /// <returns></returns>
        public int AddToGlobalInventory(ResourceAmount resAmount) {
            return AddToGlobalInventory(resAmount.resourceType, resAmount.resourceAmount);
        }

        /// <summary>
        /// Check if the specific amount for that resource is available
        /// </summary>
        /// <param name="res"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool HasResInGlobalInventory(ResourceType res, int amount) {
            globalInventory.TryGetValue(res, out int currentAmount);
            return currentAmount >= amount;
        }

        /// <summary>
        /// Remove the specified resourceAmount from inventory
        /// </summary>
        /// <param name="resAmount"></param>
        /// <returns></returns>
        public bool RemoveResFromGlobalInventory(ResourceAmount resAmount) {
            return RemoveResFromGlobalInventory(resAmount.resourceType, resAmount.resourceAmount);
        }

        /// <summary>
        /// Remove the specified amount of resourceType
        /// </summary>
        /// <param name="res"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool RemoveResFromGlobalInventory(ResourceType res, int amount) {
            Assert.IsTrue(HasResInGlobalInventory(res, amount));
            globalInventory.TryGetValue(res, out int currentAmount);
            int newAmount = globalInventory[res] = currentAmount - amount;
            TriggerInventoryChanged();
            return newAmount >= 0;
        }

        /// <summary>
        /// Check if the building cost resources are in the global inventory
        /// </summary>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        public bool HasEnoughResourcesToBuildBuilding(BuildingType buildingType) {
            Entity buildingEntityPrefab = assetManager.GetBuildingEntityPrefab(buildingType);
            BuildingComponent buildingComponent = entityManager.GetComponentData<BuildingComponent>(buildingEntityPrefab);
            bool enoughWood = HasResInGlobalInventory(ResourceType.Wood, buildingComponent.buildingCosts.wood);
            bool enoughStone = HasResInGlobalInventory(ResourceType.Stone, buildingComponent.buildingCosts.stone);
            bool result = enoughWood && enoughStone;
            return result;
        }

        /// <summary>
        /// Try to remove building Costs from Inventory.
        /// Return true if it worked, false if not
        /// </summary>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        public bool TryToRemoveBuildingCostsFromInventory(BuildingType buildingType) {
            if (!HasEnoughResourcesToBuildBuilding(buildingType)) {
                return false;
            }
            Entity buildingEntityPrefab = assetManager.GetBuildingEntityPrefab(buildingType);
            BuildingComponent buildingComponent = entityManager.GetComponentData<BuildingComponent>(buildingEntityPrefab);
            RemoveResFromGlobalInventory(ResourceType.Wood, buildingComponent.buildingCosts.wood);
            RemoveResFromGlobalInventory(ResourceType.Stone, buildingComponent.buildingCosts.stone);

            return true;
        }

        /// <summary>
        /// Set Limit for the specified resourceType
        /// </summary>
        /// <param name="res"></param>
        /// <param name="limit"></param>
        public void SetLimit(ResourceType res, int limit) {
            globalInventoryLimits[res] = limit;
        }

        /// <summary>
        /// Check if the limit for the specified resourceType is reached
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public bool IsLimitReached(ResourceType res) {
            return globalInventory[res] >= globalInventoryLimits[res];
        }

        /// <summary>
        /// Get Inventory-Limit for the specified resource-type
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public int GetLimit(ResourceType res) {
            return globalInventoryLimits[res];
        }

        /// <summary>
        /// Register InventoryChanged Subscriber
        /// Optionally Trigger directly on registration
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="trigger"></param>
        public void RegisterInventoryChangedEvent(EventHandler<Dictionary<ResourceType, int>> listener, bool trigger = false) {
            EventInventoryChanged += listener;
            if (trigger) {
                TriggerInventoryChanged();
            }
        }

        /// <summary>
        /// Unregister InventoryChanged Subscriber
        /// </summary>
        /// <param name="listener"></param>
        public void UnregisterInventoryChangedEvent(EventHandler<Dictionary<ResourceType, int>> listener) {
            EventInventoryChanged -= listener;
        }


        private void TriggerInventoryChanged() {
            EventInventoryChanged?.Invoke(this, globalInventory);
        }

    }

}