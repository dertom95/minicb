using Components;
using Data;
using Manager;
using System;
using System.Collections.Generic;
using Systems.SystemGroups;
using Unity.Entities;


namespace Systems {
    /// <summary>
    /// System to load prefabs and store them in the AssetManager
    /// </summary>
    [UpdateInGroup(typeof(GameSystemGroup),OrderFirst = true)]
    public partial struct PrefabLoaderSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            // only run if there is at least one entity with BuildingPrefabBufferElement (this prevents running update before the scene is finished loading)
            state.RequireForUpdate<BuildingPrefabBufferElement>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityManager entityManager = state.EntityManager;

            state.Enabled = false;

            // Find all Building-EntityPrefabs
            foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<BuildingPrefabBufferElement>>().WithEntityAccess()) {
                foreach (var entry in buffer) {
                    Mgr.assetManager.RegisterBuildingEntityPrefab(entry.type, entry.prefabEntity);
                }
            }

            // Find all Resource-EntityPrefabs
            foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<ResourcePrefabBufferElement>>().WithEntityAccess()) {
                foreach (var entry in buffer) {
                    Mgr.assetManager.RegisterResourceEntityPrefab(entry.type, entry.prefabEntity);
                }
            }

            // Find all Settler-EntityPrefabs
            foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<SettlerPrefabBufferElement>>().WithEntityAccess()) {
                foreach (var entry in buffer) {
                    SettlerComponent settlerComponent = entityManager.GetComponentData<SettlerComponent>(entry.prefabEntity);
                    Mgr.assetManager.RegisterSettlerEntityPrefab(settlerComponent.settlerType, entry.prefabEntity);
                }
            }

            EntityUtils.InitQueries(ref state);
        }
    }
}
