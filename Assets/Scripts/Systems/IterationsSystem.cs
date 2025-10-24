using Components;
using Unity.Entities;

namespace Systems {
    public partial struct IterationsSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (iterationsComp, entity) in SystemAPI.Query<IterationsComponent>()
                                                    .WithAll<TagIterationAutoDestroy>()
                                                    .WithDisabled<JobTargetComponent>()
                                                    .WithEntityAccess()
            ) { 
                if (iterationsComp.iterationsLeft == 0) {
                    ecb.DestroyEntity(entity);   
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}