using BuildingLogic;
using Components;
using Components.Tags;
using Manager;
using Unity.Collections;
using Unity.Entities;

namespace Systems {
    /// <summary>
    /// System to iterate over all open jobs and try to assign free settlers to the jobs
    /// </summary>
    public partial struct JobDistributionSystem : ISystem {
        private EntityQuery jobQuery;
        private EntityQuery settlerQuery;

        public void OnCreate(ref SystemState state) {
            jobQuery = state.GetEntityQuery(
                ComponentType.ReadWrite<JobComponent>(),
                ComponentType.Exclude<TagWorking>()
            );

            settlerQuery = state.GetEntityQuery(
                ComponentType.ReadWrite<SettlerComponent>(),
                ComponentType.Exclude<TagWorking>()
            );
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            // Get NativeArrays of entities and components
            NativeArray<Entity> jobEntities = jobQuery.ToEntityArray(Allocator.Temp);
            NativeArray<JobComponent> jobComponents = jobQuery.ToComponentDataArray<JobComponent>(Allocator.Temp);

            NativeArray<Entity> settlerEntities = settlerQuery.ToEntityArray(Allocator.Temp);
            NativeArray<SettlerComponent> settlerComponents = settlerQuery.ToComponentDataArray<SettlerComponent>(Allocator.Temp);

            // Track settlers assigned this update to avoid multiple assignments
            NativeHashSet<Entity> assignedSettlers = new NativeHashSet<Entity>(settlerEntities.Length, Allocator.Temp);

            for (int jobIndex = 0; jobIndex < jobEntities.Length; jobIndex++) {
                Entity jobEntity = jobEntities[jobIndex];
                JobComponent job = jobComponents[jobIndex];
                
                IJobLogic jobLogic = JobManager.Instance.GetJobLogic(job.jobType);
                
                // check if the job can be assigned at all
                bool canAssignJob = jobLogic.CanAcceptJob(jobEntity, state.EntityManager, ref job);
                if (!canAssignJob) {
                    continue;
                }

                bool assigned = false;

                // Find next settler that accepts this job type and is not assigned yet
                for (int settlerIndex=0,settlerIndexEnd = settlerEntities.Length; settlerIndex < settlerIndexEnd; settlerIndex++) {
                    Entity settlerEntity = settlerEntities[settlerIndex];
                    SettlerComponent settler = settlerComponents[settlerIndex];

                    if (assignedSettlers.Contains(settlerEntity)) {
                        continue; // Already assigned this update
                    }

                    // Check bitmask compatibility
                    if ((settler.acceptJobs & job.jobType) != 0) {
                        // Assign job to settler and settler to job
                        job.jobSettler = settlerEntity;
                        job.jobState = Data.JobState.Start;

                        settler.currentJob = jobEntity;
                        settler.settlerState = Data.SettlerState.Working;

                        // Update components via ECB
                        ecb.SetComponent(jobEntity, job);
                        ecb.SetComponent(settlerEntity, settler);

                        // Add TagWorking to mark busy
                        ecb.SetComponentEnabled<TagWorking>(jobEntity, true);
                        ecb.SetComponentEnabled<TagWorking>(settlerEntity, true);

                        assignedSettlers.Add(settlerEntity);
                        assigned = true;

                        break;
                    }
                }
            }

            // Playback all structural changes
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            // Dispose NativeArrays and NativeHashSet
            jobEntities.Dispose();
            jobComponents.Dispose();
            settlerEntities.Dispose();
            settlerComponents.Dispose();
            assignedSettlers.Dispose();
        }
    }
}
