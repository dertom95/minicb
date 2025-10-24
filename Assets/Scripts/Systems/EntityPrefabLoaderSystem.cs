using Data;
using Manager;
using System;
using System.Collections.Generic;
using Unity.Entities;


namespace Systems {
    public partial struct BuildingPrefabDictionarySystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BuildingPrefabBufferElement>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityManager entityManager = state.EntityManager;

            // Find all Building-EntityPrefabs
            foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<BuildingPrefabBufferElement>>().WithEntityAccess()) {
                foreach (var entry in buffer) {
                    AssetManager.Instance.RegisterBuildingEntityPrefab(entry.type, entry.prefabEntity);
                }
            }

            // Find all Resource-EntityPrefabs
            foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<ResourcePrefabBufferElement>>().WithEntityAccess()) {
                foreach (var entry in buffer) {
                    AssetManager.Instance.RegisterResourceEntityPrefab(entry.type, entry.prefabEntity);
                }
            }

            state.Enabled = false;
        }
    }
}
