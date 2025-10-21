//using Data;
//using Manager;
//using System.Collections.Generic;
// obsolete. Doing this in the AssetManager now. But keeping it for the time being

//using Unity.Entities;

//namespace Systems {
//    public partial struct BuildingPrefabDictionarySystem : ISystem {
//        public void OnCreate(ref SystemState state) {
//        }

//        public void OnUpdate(ref SystemState state) {
//            // let the system run until prefabs are found. (Mainly this is done to work with the Unity-Tests that first need to finish loading the scene)
//            bool foundPrefabs = false;

//            // Find all building-EntityPrefabs
//            foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<BuildingPrefabBufferElement>>().WithEntityAccess()) {
//                foreach (var entry in buffer) {
//                    DataManager.Instance.RegisterBuildingEntityPrefab(entry.type, entry.prefabEntity);
//                }
//                foundPrefabs = true;
//            }

//            // Find all building-EntityPrefabs
//            foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<ResourcePrefabBufferElement>>().WithEntityAccess()) {
//                foreach (var entry in buffer) {
//                    DataManager.Instance.RegisterResourceEntityPrefab(entry.type, entry.prefabEntity);
//                }
//            }

//            if (foundPrefabs) {
//                state.Enabled = false;
//            }
//        }
//    }
//}
