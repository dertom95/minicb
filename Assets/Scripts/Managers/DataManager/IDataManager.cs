using Data;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Manager {
    /// <summary>
    /// (Game) Data related Stores
    /// TODO: Move Inventory/Limit to Manager(?) of itself
    /// </summary>
    public interface IDataManager : IManager {
        /// <summary>
        /// Add resourceAmount to inventor
        /// </summary>
        /// <param name="resAmount"></param>
        /// <returns></returns>
        int AddToGlobalInventory(ResourceAmount resAmount);
        /// <summary>
        /// Add resource to inventor
        /// </summary>
        /// <param name="res"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        int AddToGlobalInventory(ResourceType res, int amount);
        
        /// <summary>
        /// Remove resource from inventory
        /// </summary>
        /// <param name="resAmount"></param>
        /// <returns></returns>
        bool RemoveResFromGlobalInventory(ResourceAmount resAmount);

        /// <summary>
        /// Remove from inventory
        /// </summary>
        /// <param name="res"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        bool RemoveResFromGlobalInventory(ResourceType res, int amount);

        /// <summary>
        /// Is the specified amount available in inventory?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        bool HasResourceInGlobalInventory(ResourceType res, int amount);



        /// <summary>
        /// Subscribe to inventoryChanged event.
        /// Optionally trigger on subscription
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="trigger"></param>
        void SubscribeToInventoryChangedEvent(EventHandler<Dictionary<ResourceType, int>> listener, bool trigger = false);
        
        /// <summary>
        /// Unsubscribe from inventoryChanged event
        /// </summary>
        /// <param name="listener"></param>
        void UnsubscribeFromInventoryChangedEvent(EventHandler<Dictionary<ResourceType, int>> listener);

        /// <summary>
        /// Enough resouces to build a specific building 
        /// TODO: put to BuildingManager?
        /// </summary>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        bool HasEnoughResourcesToBuildBuilding(BuildingType buildingType);

        /// <summary>
        /// Remove buildingCosts for buildingType from inventory if possible (return true). If not return false
        /// </summary>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        bool TryToRemoveBuildingCostsFromInventory(BuildingType buildingType);

        /// <summary>
        /// Is the upper limit for a resource reached?
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        bool IsLimitReached(ResourceType res);

        /// <summary>
        /// Get the inventory limit for the specified resource
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        int GetLimit(ResourceType res);

        /// <summary>
        /// Set the inventory limit for the specified resource
        /// </summary>
        /// <param name="res"></param>
        /// <param name="limit"></param>
        void SetLimit(ResourceType res, int limit);

        /// <summary>
        /// Get one entity in the specified sphere.
        /// TODO: Move to somewhere else...!?
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        Entity GetResourceEntityInRadius(float3 position, float radius, ResourceType resourceType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int StoreData(object data);


        /// <summary>
        /// Get data from generic data store
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        object GetData(int objectId);

        /// <summary>
        /// Remove data
        /// </summary>
        /// <param name="objectId"></param>
        void RemoveData(int objectId);

    }
}