using BuildingLogic;
using Components;
using Components.Tags;
using Data;
using Manager;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Assertions;

namespace Systems {
    public partial struct JobLifeCycleSystem : ISystem {
        private const float SQRDISTANCE_TO_FINISH_MOVEMENT = 1f;

        private EntityQuery activeJobsQuery;
        private EntityManager entityManager;

        public void OnCreate(ref SystemState state) {
            activeJobsQuery = state.GetEntityQuery(
                ComponentType.ReadOnly<JobComponent>(),
                ComponentType.ReadOnly<TagWorking>()
            );
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
            Assert.IsTrue(job.jobSettler != Entity.Null);
            Assert.IsTrue(SystemAPI.HasComponent<SettlerComponent>(job.jobSettler));

            IJobLogic jobLogic = JobManager.Instance.GetJobLogic(job.jobType);


            //█▀ ▀█▀ ▄▀█ █▀█ ▀█▀ █▀▀ █▀▄
            //▄█ ░█░ █▀█ █▀▄ ░█░ ██▄ █▄▀
            if (job.jobState == Data.JobState.Start) {
                bool success = jobLogic.OnStarted(ref ecb, ref jobEntity, ref state, ref job, dt);
                if ( !success) {
                    // something went wrong! quit the job
                    job.jobState = JobState.Quit;
                } else {
                    job.jobState = JobState.MovingToTarget;
                    AnimationManager.Instance.SetAnimationState(job.jobSettler, AnimationState.walking);
                    
                    // rotate to direction
                    RefRW<LocalTransform> settlerTransform = SystemAPI.GetComponentRW<LocalTransform>(job.jobSettler);
                    float3 direction = math.normalize(job.jobPosition - settlerTransform.ValueRO.Position);
                    quaternion rotation = quaternion.LookRotation(direction, math.up());
                    settlerTransform.ValueRW.Rotation = rotation;
                }
                ecb.SetComponent(jobEntity, job);
            }
            //█▀▄▀█ █▀█ █░█ █ █▄░█ █▀▀   ▀█▀ █▀█   ▀█▀ ▄▀█ █▀█ █▀▀ █▀▀ ▀█▀
            //█░▀░█ █▄█ ▀▄▀ █ █░▀█ █▄█   ░█░ █▄█   ░█░ █▀█ █▀▄ █▄█ ██▄ ░█░            
            else if (job.jobState == Data.JobState.MovingToTarget || job.jobState == Data.JobState.MovingToOwner) {
                RefRW<LocalTransform> settlerTransform = SystemAPI.GetComponentRW<LocalTransform>(job.jobSettler);
                RefRW<SettlerComponent> settlerComponent = SystemAPI.GetComponentRW<SettlerComponent>(job.jobSettler);

                float maxStep = settlerComponent.ValueRO.walkSpeed * dt;
                float3 direction = math.normalize(job.jobPosition - settlerTransform.ValueRO.Position);
                float distanceSq = math.distancesq(settlerTransform.ValueRO.Position, job.jobPosition);
                
                if (distanceSq <= SQRDISTANCE_TO_FINISH_MOVEMENT) {
                    // reached target
                    job.jobState = job.jobState == JobState.MovingToTarget
                                    ? Data.JobState.ReachedTarget
                                    : Data.JobState.ReachedMovingToOwner;

                    ecb.SetComponent(jobEntity, job);
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

                RefRW<WorkTimerComponent> settlerWorkTimerComponent = SystemAPI.GetComponentRW<WorkTimerComponent>(job.jobSettler);
                
                settlerWorkTimerComponent.ValueRW.currentTime = 0;
                settlerWorkTimerComponent.ValueRW.workTimeTotal = job.jobDuration;

                job.jobState = Data.JobState.Working;
                ecb.SetComponent(jobEntity, job);

                AnimationManager.Instance.SetAnimationState(job.jobSettler, AnimationState.working);
            }
            //█░█░█ █▀█ █▀█ █▄▀ █ █▄░█ █▀▀
            //▀▄▀▄▀ █▄█ █▀▄ █░█ █ █░▀█ █▄█
            else if (job.jobState == Data.JobState.Working) {
                RefRW<WorkTimerComponent> settlerWorkTimerComponent = SystemAPI.GetComponentRW<WorkTimerComponent>(job.jobSettler);

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
                AnimationManager.Instance.SetAnimationState(job.jobSettler, AnimationState.walking); 
                
                jobLogic.OnFinishedWorking(ref ecb, ref jobEntity, ref state, ref job, dt);
                
                bool isJobFinished = !jobLogic.NeedsToGoBackToOwner();
                if (isJobFinished) {
                    // TODO: Do we want to use (new) JobState-Quit for this?
                    jobLogic.OnExit(ref ecb, ref jobEntity, ref state, ref job, dt);
                    cleanupJob(ref ecb, ref jobEntity, ref state, ref job, dt);
                } else {
                    LocalTransform ownerTransform = SystemAPI.GetComponent<LocalTransform>(job.jobOwner);
                    job.jobPosition = ownerTransform.Position;
                    job.jobState = Data.JobState.MovingToOwner;
                    ecb.SetComponent(jobEntity, job);

                    // rotate to direction
                    RefRW<LocalTransform> settlerTransform = SystemAPI.GetComponentRW<LocalTransform>(job.jobSettler);
                    float3 direction = math.normalize(job.jobPosition - settlerTransform.ValueRO.Position);
                    quaternion rotation = quaternion.LookRotation(direction, math.up());
                    settlerTransform.ValueRW.Rotation = rotation;
                }
            }
            //█▀█ █▀▀ ▄▀█ █▀▀ █░█ █▀▀ █▀▄   █▀█ █░█░█ █▄░█ █▀▀ █▀█
            //█▀▄ ██▄ █▀█ █▄▄ █▀█ ██▄ █▄▀   █▄█ ▀▄▀▄▀ █░▀█ ██▄ █▀▄
            else if (job.jobState == JobState.ReachedMovingToOwner) {
                // finished
                jobLogic.OnReachedOwner(ref ecb, ref jobEntity, ref state, ref job, dt);

                // TODO: Do we want to use (new) JobState-Quit for this?
                jobLogic.OnExit(ref ecb, ref jobEntity, ref state, ref job, dt);
                cleanupJob(ref ecb, ref jobEntity, ref state, ref job, dt);
            }
            else if (job.jobState == JobState.Quit) {
                jobLogic.OnExit(ref ecb, ref jobEntity, ref state, ref job, dt);
                cleanupJob(ref ecb, ref jobEntity, ref state, ref job, dt);
            }
        }

        private void cleanupJob(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
            AnimationManager.Instance.SetAnimationState(job.jobSettler, AnimationState.idle);

            // remove job reference in settler and make settler available
            if (job.jobType != JobType.Construction) {
                RefRW<JobEmitterComponent> jobEmitterComponent = SystemAPI.GetComponentRW<JobEmitterComponent>(job.jobOwner);
                jobEmitterComponent.ValueRW.currentJobAmount--;
                
                SystemAPI.SetComponentEnabled<TagCreateJobs>(job.jobOwner, true);
            }
            
            ecb.SetComponentEnabled<TagWorking>(job.jobSettler, false);
            RefRW<SettlerComponent> settlerComponent = SystemAPI.GetComponentRW<SettlerComponent>(job.jobSettler);
            settlerComponent.ValueRW.currentJob = Entity.Null;
            ecb.DestroyEntity(jobEntity);
        }
    }
}