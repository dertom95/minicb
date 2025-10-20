using BuildingLogic;
using Components;
using Components.Tags;
using Data;
using NUnit.Framework;
using System;
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

        private Dictionary<JobType, IJobLogic> jobLogicLookup;
        private Dictionary<BuildingType, Func<Entity, Entity>> buildingJobSpawner;

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
            InitJobLogic();
        }

        /// <summary>
        /// Initialize joblogic that is executed in the JobLifeCycleSystem
        /// </summary>
        private void InitJobLogic() {
            jobLogicLookup = new Dictionary<JobType, IJobLogic>();
            jobLogicLookup.Add(JobType.Construction, new JobConstruction());
        }

        private void InitJobSpawner() {
            buildingJobSpawner = new Dictionary<BuildingType, Func<Entity, Entity>>();
            
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

        /// <summary>
        /// Creates a ConstructionJob-Entity
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public Entity CreateEntityToResourceJob(Entity owner) {
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

        /// <summary>
        /// Return job specific logic to be executed in JobLifeCycleSystem
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        // TODO: Move this to the system? 
        public IJobLogic GetJobLogic(JobType jobType) {
            Assert.IsTrue(jobLogicLookup.ContainsKey(jobType));

            return jobLogicLookup[jobType];
        }

    }
}