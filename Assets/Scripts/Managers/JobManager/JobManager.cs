using BuildingLogic;
using Components;
using Components.Tags;
using Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Assertions;

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
            jobLogicLookup.Add(JobType.EntityToResource, new JobEntityToResource());
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

            BuildingComponent buildingComponent = entityManager.GetComponentData<BuildingComponent>(owner);

            Entity constructionJob = CreateGenericJob(
                owner, 
                owner, 
                JobType.Construction, 
                buildingComponent.buildingCosts.timeInSeconds
            );

            return constructionJob;
        }

        /// <summary>
        /// Creates a ConstructionJob-Entity
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public Entity CreateGenericJob(Entity owner, Entity jobTarget, JobType jobType, float duration, EntityCommandBuffer? ecb=null) {
            Assert.AreNotEqual(Entity.Null, owner);

            Entity jobEntity = entityManager.CreateEntity();

            LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(jobTarget);

            if (ecb.HasValue) {
                ecb.Value.AddComponent(jobEntity, new JobComponent {
                    jobOwner = owner,
                    jobTarget = jobTarget,
                    jobPosition = localTransform.Position,
                    jobType = jobType,
                    jobState = JobState.MovingToTarget,
                    jobDuration = duration
                });

                ecb.Value.AddComponent<TagWorking>(jobEntity);
                ecb.Value.SetComponentEnabled<TagWorking>(jobEntity, false);
            } else {
                entityManager.AddComponentData(jobEntity, new JobComponent {
                    jobOwner = owner,
                    jobTarget = jobTarget,
                    jobPosition = localTransform.Position,
                    jobType = jobType,
                    jobState = JobState.MovingToTarget,
                    jobDuration = duration
                });

                entityManager.AddComponent<TagWorking>(jobEntity);
                entityManager.SetComponentEnabled<TagWorking>(jobEntity, false);

            }

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