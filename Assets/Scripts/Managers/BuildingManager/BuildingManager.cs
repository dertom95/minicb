using Components;
using Components.Tags;
using Data;
using NUnit.Framework;
using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Manager {
    /// <summary>
    /// Building-Specific functions
    /// </summary>
    public class BuildingManager : IManager {
        public const float BUILDING_CONSTRUCTIONSITE_SCALE = 0.2f;

        private static readonly BuildingManager instance = new BuildingManager();

        public static BuildingManager Instance => instance;

        private EntityManager entityManager;
        private DataManager dataManager;

        private BuildingManager() {
        }

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            dataManager = DataManager.Instance;
        }

        /// <summary>
        /// Spawn building with position/rotation(degree)/scale
        /// </summary>
        /// <param name="buildingType"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        public Entity SpawnBuilding(BuildingType buildingType, float3 position, float scale, float3 rotation, bool immediateBuild = false) {
            Entity entityPrefab = dataManager.GetBuildingEntityPrefab(buildingType);
            Assert.IsNotNull(entityPrefab);
            Entity newEntity = entityManager.Instantiate(entityPrefab);

            scale = immediateBuild ? scale : math.min(scale, BUILDING_CONSTRUCTIONSITE_SCALE);

            LocalTransform newTransform = new LocalTransform {
                Position = position,
                Rotation = quaternion.EulerXYZ(new float3(math.radians(rotation.x), math.radians(rotation.y), math.radians(rotation.z))),
                Scale = scale
            };
            entityManager.SetComponentData(newEntity, newTransform);

            if (immediateBuild == false) {
                entityManager.AddComponent<TagUnderConstruction>(newEntity);
                 
                BuildingComponent buildingComponent = entityManager.GetComponentData<BuildingComponent>(newEntity);
                JobManager.Instance.CreateConstructionJob(newEntity);
                // TODO: Create Construction Job
            } else {
                throw new System.Exception("immediateBuild,not implemented!");
            }

            return newEntity;
        } 

        /// <summary>
        /// Spawn building at position
        /// </summary>
        /// <param name="buildingType"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public Entity SpawnBuilding(BuildingType buildingType, float3 position) {
            return SpawnBuilding(buildingType, position, 1.0f, new float3(0, 45f, 0));
        }


        public void Update(float dt) {
        }

        public void Dispose() {
        }
    }
}