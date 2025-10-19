using Data;
using Manager;
using System.Collections.Generic;
using Unity.Entities;

public partial struct BuildingPrefabDictionarySystem : ISystem {
    public void OnCreate(ref SystemState state) {
    }

    public void OnUpdate(ref SystemState state) {
        // let the system run until prefabs are found. (Mainly this is done to work with the Unity-Tests that first need to finish loading the scene)
        bool foundPrefabs = false;

        // Find all building-EntityPrefabs
        foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<BuildingPrefabBufferElement>>().WithEntityAccess()) {
            foreach (var entry in buffer) {
                DataManager.Instance.RegisterBuildingEntityPrefab(entry.type, entry.prefabEntity);
            }
            foundPrefabs = true;
        }

        if (foundPrefabs) {
            state.Enabled = false;
        }
    }
}
