using Components;
using Components.Decorators;
using Components.Tags;
using Data;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine.Assertions;

namespace Manager {
    /// <summary>
    /// Building-Specific functions
    /// </summary>
    public class BuildingManager : IManager, IBuildingManager {
        public const float BUILDING_CONSTRUCTIONSITE_SCALE = 0.2f;

        private EntityManager entityManager;
        private IDataManager dataManager;
        private IAssetManager assetManager;

        public BuildingManager() {
        }

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            dataManager = Mgr.dataManager;
            assetManager = Mgr.assetManager;
        }



        /// <summary>
        /// Spawn building with position/rotation(degree)/scale
        /// </summary>
        /// <param name="buildingType"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        public Entity SpawnBuilding(BuildingType buildingType, float3 position, float scale, float3 rotation, bool immediateBuild = false, bool createConstructionJobs = true) {
            Entity entityPrefab = assetManager.GetBuildingEntityPrefab(buildingType);
            Assert.IsFalse(entityPrefab == Entity.Null);
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
                if (createConstructionJobs) {
                    Mgr.jobManager.CreateConstructionJob(newEntity);
                }
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
        public Entity SpawnBuilding(BuildingType buildingType, float3 position, bool immediateBuild=false, bool createConstructionJobs=true) {
            return SpawnBuilding(buildingType, position, 1.0f, new float3(0, 45f, 0),immediateBuild,createConstructionJobs);
        }

        /// <summary>
        /// Spawns a preview building that got decorated components stripped away
        /// </summary>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        public Entity SpawnPreviewBuilding(BuildingType buildingType, bool removePhysicsOnToplevelEntity = true) {
            Entity newBuildingEntity = SpawnBuilding(buildingType, default, createConstructionJobs: false);

            Assert.IsTrue(newBuildingEntity != Entity.Null);

            EntityTraversalUtility.TraverseEntityHierarchy(newBuildingEntity, entityManager, (entity, em) => {
                // yes, I know! That is anti-performant. But it is a once per click event and for now I will go for it. I could cache it per BuildingType
                List<System.Type> compTypes = new List<System.Type>() { 
                };
                if (removePhysicsOnToplevelEntity) {
                    compTypes.Add(typeof(PhysicsCollider));
                }

                var componentTypes = em.GetComponentTypes(entity);
                foreach (ComponentType componentType in componentTypes) {
                    Type compType = TypeManager.GetType(componentType.TypeIndex);

                    if (compType == null || !(typeof(IRemoveForPreviewEntity).IsAssignableFrom(compType)) ) {
                        continue;
                    }
                    compTypes.Add(compType);
                }

                foreach (Type componentType in compTypes) {
                    if (em.HasComponent(newBuildingEntity, componentType)) {
                        em.RemoveComponent(newBuildingEntity, componentType);
                    }
                }

                return false;
            });

            return newBuildingEntity;
        }

        /// <summary>
        /// Enable/Disable building 
        /// </summary>
        /// <param name="buildingEntity"></param>
        /// <param name="enabled"></param>
        public void SetBuildingEnabled(Entity buildingEntity, bool enabled) {
            Assert.IsTrue(buildingEntity != Entity.Null);


        }


        public void Update(float dt) {
        }

        public void Dispose() {
        }
    }
}