using Components;
using Components.Tags;
using Manager;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Assertions;

namespace Systems {
    public partial struct JobCreationSystem : ISystem {

        private static Random random = new Random();

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            HandleJobCreation(ref ecb, ref state);

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        /// <summary>
        /// Handle job creation for all JobEmitters
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="state"></param>
        private void HandleJobCreation(ref EntityCommandBuffer ecb, ref SystemState state) {
            // Query entities with the specified components and tags.
            // TODO: Still looking for a good readable format to write those queries
            foreach (
                (
                    RefRW<JobEmitterComponent> jobEmitterComp,
                    LocalTransform localTransform,
                    Entity buildingEntity
                )
                in SystemAPI.Query<RefRW<JobEmitterComponent>,LocalTransform>()
                .WithAll<TagWorking, TagBuilt, TagCreateJobs>()
                .WithEntityAccess())
            {
                // Check job amount constraint
                Assert.IsTrue(jobEmitterComp.ValueRO.currentJobAmount < jobEmitterComp.ValueRO.maxJobs);

                Entity jobTarget = Entity.Null;

                // EntityToResource-specific
                if (   SystemAPI.HasComponent<EntityToResourceComponent>(buildingEntity)
                    && SystemAPI.HasComponent<FieldOfInfluenceComponent>(buildingEntity)
                ) {
                    // find a valid resource at job-creation
                    EntityToResourceComponent entityToResourceComp = SystemAPI.GetComponent<EntityToResourceComponent>(buildingEntity);
                    FieldOfInfluenceComponent fieldOfInfluenceComp = SystemAPI.GetComponent<FieldOfInfluenceComponent>(buildingEntity);

                    // search for specific resource
                    Entity resourceEntity = Mgr.dataManager.GetResourceEntityInRadius(
                        localTransform.Position,
                        fieldOfInfluenceComp.radius,
                        entityToResourceComp.searchResourceType
                    );

                    if (resourceEntity != Entity.Null) {
                        RefRW<IterationsComponent> iterationsComponent = SystemAPI.GetComponentRW<IterationsComponent>(resourceEntity);
                        Assert.IsTrue(iterationsComponent.ValueRO.iterationsLeft > 0);

                        iterationsComponent.ValueRW.iterationsLeft--;

                        if (SystemAPI.HasComponent<JobTargetComponent>(resourceEntity)) {
                            EntityManager em = state.EntityManager;
                            Mgr.jobManager.PendingJobsIncrease(resourceEntity,ref em);
                        }
                    }
 
                    jobTarget = resourceEntity;
                } else if (SystemAPI.HasComponent<ResourceToResourceComponent>(buildingEntity)) {
                    jobTarget = buildingEntity;
                } else if (SystemAPI.HasComponent<SpawnEntityComponent>(buildingEntity)){
                    jobTarget = buildingEntity; // for now set target to the building! Later set jobEntity's position to the spawnPosition and use target
                } else {
                    Assert.IsTrue(false, "Couldn't determine the buildingType");
                }

                if (jobTarget != Entity.Null) {
                    // found a resource entity in my radius
                    Mgr.jobManager.CreateGenericJob(
                        owner: buildingEntity,
                        jobTarget: jobTarget,
                        jobType: jobEmitterComp.ValueRO.jobType,
                        duration: jobEmitterComp.ValueRO.jobDurationInSeconds,
                        ecb: ecb
                    );

                    jobEmitterComp.ValueRW.currentJobAmount++;

                    bool reachedMaxJobAmount = jobEmitterComp.ValueRW.currentJobAmount == jobEmitterComp.ValueRO.maxJobs;
                    if (reachedMaxJobAmount) {
                        ecb.SetComponentEnabled<TagCreateJobs>(buildingEntity, false);
                    }
                }
            }
        }

    }
}
