using Manager;
using Unity.Collections;
using Unity.Entities;

namespace Manager {
    public class AssetManager : IManager, IManagerUpdateable {
        private static AssetManager instance = new AssetManager();
        public static AssetManager Instance => instance;
        
        private AssetManager() { }

        bool foundAssets = false;

        EntityManager entityManager;

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public void Dispose() {
        }

        public void Update(float dt) {
            if (!foundAssets) {
                foundAssets = TryToRegisterPrefabs(entityManager);
            }
        }

        public bool TryToRegisterPrefabs(EntityManager entityManager) {
            bool found = false;
            // Query for BuildingPrefabBufferElement buffers
            var buildingQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<BuildingPrefabBufferElement>());

            DynamicBuffer<BuildingPrefabBufferElement> buildingPrefabBuffer = buildingQuery.GetSingletonBuffer<BuildingPrefabBufferElement>();
            
            foreach (var entry in buildingPrefabBuffer) {
                DataManager.Instance.RegisterBuildingEntityPrefab(entry.type, entry.prefabEntity);
                found = true;
            }
            
            // Query for ResourcePrefabBufferElement buffers
            var resourceQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ResourcePrefabBufferElement>());

            var resourcePrefabBuffer = resourceQuery.GetSingletonBuffer<ResourcePrefabBufferElement>();

            foreach (var entry in resourcePrefabBuffer) {
                DataManager.Instance.RegisterResourceEntityPrefab(entry.type, entry.prefabEntity);
            }

            return found;
        }


    }
}


