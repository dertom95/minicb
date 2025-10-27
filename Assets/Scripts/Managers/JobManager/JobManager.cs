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
    public class JobManager : IJobManager {
        private Dictionary<JobType, IJobLogic> jobLogicLookup;

        private EntityManager entityManager;
        private IDataManager dataManager;
        private EntityQuery jobComponentQuery;
        private EntityQuery settlerComponentQuery;

        private EntityArchetype archetypeJob;

        public JobManager() {
        }

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            dataManager = Mgr.dataManager;
            jobComponentQuery = entityManager.CreateEntityQuery(typeof(JobComponent));
            archetypeJob = entityManager.CreateArchetype(typeof(LocalTransform), typeof(JobComponent));

            InitJobLogic();
        }

        /// <summary>
        /// Initialize joblogic that is executed in the JobLifeCycleSystem
        /// </summary>
        private void InitJobLogic() {
            jobLogicLookup = new Dictionary<JobType, IJobLogic>();
            jobLogicLookup.Add(JobType.Construction, new JobConstruction());
            jobLogicLookup.Add(JobType.EntityToResource, new JobEntityToResource());
            jobLogicLookup.Add(JobType.ConvertResource, new JobResourceToResource());
            jobLogicLookup.Add(JobType.SpawnEntity, new JobSpawnEntity());
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
        public Entity CreateGenericJob(Entity owner, Entity jobTarget, JobType jobType, float duration, EntityCommandBuffer? ecb = null) {
            Assert.AreNotEqual(Entity.Null, owner);

            Entity jobEntity;

            LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(jobTarget);

            if (ecb.HasValue) {
                jobEntity = ecb.Value.CreateEntity(archetypeJob);
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
                jobEntity = entityManager.CreateEntity(archetypeJob);

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
        /// Decrease the pending job-amount. Makes sure to disable JobTargetComponent once no job is pending anymore
        /// 
        /// Only modify pending-job amount via JobManager
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="entity"></param>
        /// <param name="em"></param>
        /// <returns></returns>
        public bool PendingJobsDecrease(Entity entity, ref EntityManager em, EntityCommandBuffer? ecb = null) {
            JobTargetComponent resComp = em.GetComponentData<JobTargetComponent>(entity);
            resComp.pendingJobs--;
            bool hasPendingJobs = resComp.pendingJobs > 0;
            if (ecb.HasValue) {
                ecb.Value.SetComponent(entity, resComp);
            } else {
                em.SetComponentData(entity, resComp);
            }
            if (!hasPendingJobs) {
                if (ecb.HasValue) {
                    ecb.Value.SetComponentEnabled<JobTargetComponent>(entity, false);
                } else {
                    em.SetComponentEnabled<JobTargetComponent>(entity, false);
                }
            }

            return hasPendingJobs;
        }

        /// <summary>
        /// Increase the pending job-amount. Makes sure to enable JobTargetComponent if needed
        ///
        /// Only modify pending-job amount via JobManager
        /// </summary>
        /// <param name="resourceEntity"></param>
        /// <param name="em"></param>
        public void PendingJobsIncrease(Entity resourceEntity, ref EntityManager em, EntityCommandBuffer? ecb = null) {
            JobTargetComponent jobTargetComp = em.GetComponentData<JobTargetComponent>(resourceEntity);

            byte pendingJobsBefore = jobTargetComp.pendingJobs;
            jobTargetComp.pendingJobs++;
            if (ecb.HasValue) {
                ecb.Value.SetComponent(resourceEntity, jobTargetComp);
            } else {
                em.SetComponentData(resourceEntity, jobTargetComp);
            }

            if (pendingJobsBefore == 0) {
                if (ecb.HasValue) {
                    ecb.Value.SetComponentEnabled<JobTargetComponent>(resourceEntity, true);
                } else {
                    em.SetComponentEnabled<JobTargetComponent>(resourceEntity, true);
                }
            }
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