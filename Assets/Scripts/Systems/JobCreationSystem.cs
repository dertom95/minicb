using Components;
using Components.Tags;
using Manager;
using NUnit.Framework;
using Unity.Entities;
using Unity.Transforms;

namespace Systems {
    public partial struct JobCreationSystem : ISystem {

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            ProcessEntityToResourceJob(ref ecb, ref state);

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        /// <summary>
        /// Find EntityToResource-Buildings that needs to create jobs
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="state"></param>
        private void ProcessEntityToResourceJob(ref EntityCommandBuffer ecb, ref SystemState state) {
            // Query entities with the specified components and tags.
            // TODO: Still looking for a good readable format to write those queries
            foreach (
                (
                    RefRW<BuildingComponent> buildingComp,
                    EntityToResourceComponent entityToResourceComp,
                    FieldOfInfluenceComponent fieldOfInfluenceComp,
                    LocalTransform localTransform,
                    Entity entity
                )
                in SystemAPI.Query<
                    RefRW<BuildingComponent>,
                    EntityToResourceComponent,
                    FieldOfInfluenceComponent,
                    LocalTransform
                >()
                .WithAll<TagWorking, TagBuilt, TagCreateJobs>()
                .WithEntityAccess()
            ) {
                // Check job amount constraint
                Assert.IsTrue(buildingComp.ValueRO.currentJobAmount < buildingComp.ValueRO.maxJobs);

                // search for specific resource
                Entity resourceEntity = DataManager.Instance.GetResourceEntityInRadius(
                    localTransform.Position,
                    fieldOfInfluenceComp.radius,
                    entityToResourceComp.searchResourceType,
                    decreaseIterations: true
                );

                if (resourceEntity != Entity.Null) {
                    // found a resource entity in my radius
                    JobManager.Instance.CreateGenericJob(
                        owner: entity,
                        jobTarget: resourceEntity,
                        jobType: buildingComp.ValueRO.jobType,
                        duration: buildingComp.ValueRO.jobDurationInSeconds,
                        ecb: ecb
                    );

                    buildingComp.ValueRW.currentJobAmount++;

                    bool reachedMaxJobAmount = buildingComp.ValueRW.currentJobAmount == buildingComp.ValueRO.maxJobs;
                    if (reachedMaxJobAmount) {
                        ecb.SetComponentEnabled<TagCreateJobs>(entity, false);
                    }
                }
            }
        }

    }
}
