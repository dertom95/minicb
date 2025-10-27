using Data;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Manager {
    public interface IDataManager : IManager {
        int AddToGlobalInventory(ResourceAmount resAmount);
        int AddToGlobalInventory(ResourceType res, int amount);
        object GetData(ushort objectId);
        int GetLimit(ResourceType res);
        Entity GetResourceEntityInRadius(float3 position, float radius, ResourceType resourceType);
        bool HasEnoughResourcesToBuildBuilding(BuildingType buildingType);
        bool HasResInGlobalInventory(ResourceType res, int amount);
        bool IsLimitReached(ResourceType res);
        void RegisterInventoryChangedEvent(EventHandler<Dictionary<ResourceType, int>> listener, bool trigger = false);
        void RemoveData(ushort objectId);
        bool RemoveResFromGlobalInventory(ResourceAmount resAmount);
        bool RemoveResFromGlobalInventory(ResourceType res, int amount);
        void SetLimit(ResourceType res, int limit);
        int StoreData(object data);
        bool TryToRemoveBuildingCostsFromInventory(BuildingType buildingType);
        void UnregisterInventoryChangedEvent(EventHandler<Dictionary<ResourceType, int>> listener);
    }
}