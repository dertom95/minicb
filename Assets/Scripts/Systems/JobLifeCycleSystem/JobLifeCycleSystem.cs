using BuildingLogic;
using Components;
using Components.Tags;
using Data;
using Manager;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems {
    public partial struct JobLifeCycleSystem : ISystem {
        private const float SQRDISTANCE_TO_FINISH_MOVEMENT = 12;
        //private static HashSet<JobType> finishedAfterWorkState;

        private EntityQuery activeJobsQuery;
        private EntityManager entityManager;

        public void OnCreate(ref SystemState state) {
            //finishedAfterWorkState = new HashSet<JobType>();
            //finishedAfterWorkState.Add(JobType.Construction);
            //finishedAfterWorkState.Add(JobType.Attack);
            //finishedAfterWorkState.Add(JobType.ConvertResource);

            activeJobsQuery = state.GetEntityQuery(
                ComponentType.ReadOnly<JobComponent>(),
                ComponentType.ReadOnly<TagWorking>());
        }
        public void OnUpdate(ref SystemState state) {
            float deltaTime = SystemAPI.Time.DeltaTime;

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            // query components and its entities
            NativeArray<JobComponent> activeJobs = activeJobsQuery.ToComponentDataArray<JobComponent>(Allocator.Temp);
            NativeArray<Entity> activeJobEntities = activeJobsQuery.ToEntityArray(Allocator.Temp);
           
            for (int i = 0, iEnd = activeJobs.Length; i < iEnd; i++) {
                JobComponent jobComponent = activeJobs[i];
                Entity entity = activeJobEntities[i];
                HandleJob(ref ecb, ref entity, ref state, ref jobComponent, deltaTime);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            activeJobs.Dispose();
            activeJobEntities.Dispose();
        }

        /// <summary>
        /// Big Job LifeCycle handling - method.
        /// Using a straight forward if/else-check due to performance reasons. It can get a bit loaded therefore I use some "bigfonts" to make it easier to navigate the code
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="state"></param>
        /// <param name="job"></param>
        /// <param name="dt"></param>
        private void HandleJob(ref EntityCommandBuffer ecb,ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
            Assert.IsTrue(job.jobWorker != Entity.Null);
            Assert.IsTrue(SystemAPI.HasComponent<SettlerComponent>(job.jobWorker));

            IJobLogic jobLogic = JobManager.Instance.GetJobLogic(job.jobType);

            //█▀▄▀█ █▀█ █░█ █ █▄░█ █▀▀   ▀█▀ █▀█   ▀█▀ ▄▀█ █▀█ █▀▀ █▀▀ ▀█▀
            //█░▀░█ █▄█ ▀▄▀ █ █░▀█ █▄█   ░█░ █▄█   ░█░ █▀█ █▀▄ █▄█ ██▄ ░█░            
            if (job.jobState == Data.JobState.MovingToTarget || job.jobState == Data.JobState.MovingToOwner) {
                RefRW<LocalTransform> settlerTransform = SystemAPI.GetComponentRW<LocalTransform>(job.jobWorker);
                RefRW<SettlerComponent> settlerComponent = SystemAPI.GetComponentRW<SettlerComponent>(job.jobWorker);

                float maxStep = settlerComponent.ValueRO.walkSpeed * dt;
                float3 direction = math.normalize(job.jobPosition - settlerTransform.ValueRO.Position);
                float distanceSq = math.distancesq(settlerTransform.ValueRO.Position, job.jobPosition);
                
                if (distanceSq <= SQRDISTANCE_TO_FINISH_MOVEMENT) {
                    // reached target
                    job.jobState = job.jobState == JobState.MovingToTarget
                                    ? Data.JobState.ReachedTarget
                                    : Data.JobState.ReachedMovingToOwner;

                    ecb.SetComponent(jobEntity, job);
                    // TODO: Do I want to make the last step? Or keep some distance to not walk into the building? :thinking:
                    //float3 newPosition = settlerTransform.ValueRO.Position + direction * math.sqrt(distanceSq);
                    //settlerTransform.ValueRW.Position = newPosition;
                } else {
                    // still moving
                    float3 newPosition = settlerTransform.ValueRO.Position + direction * maxStep;
                    settlerTransform.ValueRW.Position = newPosition;
                }
            } 
            //█▀█ █▀▀ ▄▀█ █▀▀ █░█ █▀▀ █▀▄   ▀█▀ ▄▀█ █▀█ █▀▀ █▀▀ ▀█▀
            //█▀▄ ██▄ █▀█ █▄▄ █▀█ ██▄ █▄▀   ░█░ █▀█ █▀▄ █▄█ ██▄ ░█░    
            else if (job.jobState == Data.JobState.ReachedTarget) {
                jobLogic.OnReachedTarget(ref ecb, ref jobEntity, ref state, ref job, dt);

                RefRW<WorkTimerComponent> settlerWorkTimerComponent = SystemAPI.GetComponentRW<WorkTimerComponent>(job.jobWorker);
                
                settlerWorkTimerComponent.ValueRW.currentTime = 0;
                settlerWorkTimerComponent.ValueRW.workTimeTotal = job.jobDuration;

                job.jobState = Data.JobState.Working;
                ecb.SetComponent(jobEntity, job);
            }
            //█░█░█ █▀█ █▀█ █▄▀ █ █▄░█ █▀▀
            //▀▄▀▄▀ █▄█ █▀▄ █░█ █ █░▀█ █▄█
            else if (job.jobState == Data.JobState.Working) {
                RefRW<WorkTimerComponent> settlerWorkTimerComponent = SystemAPI.GetComponentRW<WorkTimerComponent>(job.jobWorker);

                float newTime = settlerWorkTimerComponent.ValueRO.currentTime + dt;
                if (newTime >= settlerWorkTimerComponent.ValueRO.workTimeTotal) {
                    // finished work
                    job.jobState = Data.JobState.FinishedWorking;
                    ecb.SetComponent(jobEntity, job);
                } else {
                    settlerWorkTimerComponent.ValueRW.currentTime = newTime;
                    float progress = newTime / settlerWorkTimerComponent.ValueRO.workTimeTotal;

                    // specific progress-logic
                    jobLogic.OnWorking(ref ecb, ref jobEntity, ref state, ref job, dt, progress);
                }
            }
            //█▀▀ █ █▄░█ █ █▀ █░█ █▀▀ █▀▄   █░█░█ █▀█ █▀█ █▄▀ █ █▄░█ █▀▀
            //█▀░ █ █░▀█ █ ▄█ █▀█ ██▄ █▄▀   ▀▄▀▄▀ █▄█ █▀▄ █░█ █ █░▀█ █▄█
            else if (job.jobState == Data.JobState.FinishedWorking) {
                bool isJobFinished = !jobLogic.NeedsToGoBackToOwner();
                if (isJobFinished) {
                    // remove job reference in settler and make settler available
                    ecb.SetComponentEnabled<TagWorking>(job.jobWorker, false);
                    RefRW<SettlerComponent> settlerComponent = SystemAPI.GetComponentRW<SettlerComponent>(job.jobWorker);
                    settlerComponent.ValueRW.currentJob = Entity.Null;
                    ecb.DestroyEntity(jobEntity);
                } else {
                    LocalTransform ownerTransform = SystemAPI.GetComponent<LocalTransform>(job.jobWorker);
                    job.jobPosition = ownerTransform.Position;
                    job.jobState = Data.JobState.MovingToOwner;
                    ecb.SetComponent(jobEntity, job);
                }
            }
            //█▀▄▀█ █▀█ █░█ █ █▄░█ █▀▀   ▀█▀ █▀█   ▀█▀ ▄▀█ █▀█ █▀▀ █▀▀ ▀█▀
            //█░▀░█ █▄█ ▀▄▀ █ █░▀█ █▄█   ░█░ █▄█   ░█░ █▀█ █▀▄ █▄█ ██▄ ░█░            
            else if (job.jobState == Data.JobState.MovingToOwner) {
                RefRW<LocalTransform> settlerTransform = SystemAPI.GetComponentRW<LocalTransform>(job.jobWorker);
                RefRW<SettlerComponent> settlerComponent = SystemAPI.GetComponentRW<SettlerComponent>(job.jobWorker);

                float maxStep = settlerComponent.ValueRO.walkSpeed * dt;
                float3 direction = math.normalize(job.jobPosition - settlerTransform.ValueRO.Position);
                float distanceSq = math.distancesq(settlerTransform.ValueRO.Position, job.jobPosition);

                if (distanceSq <= SQRDISTANCE_TO_FINISH_MOVEMENT) {
                    // reached target
                    job.jobState = Data.JobState.ReachedTarget;
                    ecb.SetComponent(jobEntity, job);
                } else {
                    // still moving
                    float3 newPosition = settlerTransform.ValueRO.Position + direction * maxStep;
                    settlerTransform.ValueRW.Position = newPosition;
                }
            }
            else if (job.jobState == JobState.ReachedMovingToOwner) {
                // finished
                ecb.SetComponentEnabled<TagWorking>(job.jobWorker, false);
                RefRW<SettlerComponent> settlerComponent = SystemAPI.GetComponentRW<SettlerComponent>(job.jobWorker);
                settlerComponent.ValueRW.currentJob = Entity.Null;
                ecb.DestroyEntity(jobEntity);
            }
        }

        private void OnJobFinished(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt, float progress) {
            if (job.jobType == Data.JobType.Construction) {
                RefRW<LocalTransform> settlerTransform = SystemAPI.GetComponentRW<LocalTransform>(job.jobOwner);
                settlerTransform.ValueRW.Scale = math.max(BuildingManager.BUILDING_CONSTRUCTIONSITE_SCALE, progress);
            }
        }
    }
}