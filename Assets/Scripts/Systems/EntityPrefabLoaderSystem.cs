using Data;
using Manager;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.Rendering.Universal.Internal;

public partial struct BuildingPrefabDictionarySystem : ISystem {
    public void OnCreate(ref SystemState state) {
    }

    public void OnUpdate(ref SystemState state) {
        // Find all building-EntityPrefabs
        foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<BuildingPrefabBufferElement>>().WithEntityAccess()) {
            foreach (var entry in buffer) {
                DataManager.Instance.RegisterBuildingEntityPrefab(entry.type, entry.prefabEntity);
            }
        }
        state.Enabled = false;
    }
}
