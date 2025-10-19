using Components;
using Components.Tags;
using Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Manager {
    /// <summary>
    /// Building-Specific functions
    /// </summary>
    public class JobManager : IManager {
        private static readonly JobManager instance = new JobManager();

        public static JobManager Instance => instance;

        private EntityManager entityManager;
        private DataManager dataManager;
        private EntityQuery jobComponentQuery;
        private EntityQuery settlerComponentQuery;

        private JobManager() {
        }

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            dataManager = DataManager.Instance;
            jobComponentQuery = entityManager.CreateEntityQuery(typeof(JobComponent));
        }

        public void Dispose() {
        }

        /// <summary>
        /// Creates a ConstructionJob-Entity
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public Entity CreateConstructionJob(Entity owner) {
            Assert.AreNotEqual(Entity.Null, owner);

            Entity jobEntity = entityManager.CreateEntity();

            LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(owner);
            BuildingComponent buildingComponent = entityManager.GetComponentData<BuildingComponent>(owner);

            entityManager.AddComponentData(jobEntity, new JobComponent { 
                jobOwner = owner,
                jobPosition = localTransform.Position,
                jobType = JobType.Construction,
                jobState = JobState.MovingToTarget,
                jobDuration = buildingComponent.buildingCosts.timeInSeconds
            });


            entityManager.AddComponent<TagWorking>(jobEntity);
            entityManager.SetComponentEnabled<TagWorking>(jobEntity, false);

            return jobEntity;
        }

    }
}