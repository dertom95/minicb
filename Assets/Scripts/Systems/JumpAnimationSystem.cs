using Components;
using Systems.SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems {
    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct JumpAnimationSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            float dt = (float)SystemAPI.Time.DeltaTime;
            
            foreach (var (localTransform,jumpAnimComp)
                     in SystemAPI.Query<RefRW<LocalTransform>, 
                                        RefRW<JumpAnimationComponent>>()) 
            {
                float yOffset = math.abs(math.sin(jumpAnimComp.ValueRO.time)) * jumpAnimComp.ValueRO.height;
                // progress
                jumpAnimComp.ValueRW.time += dt * jumpAnimComp.ValueRO.currentSpeed;

                float3 currentPosition = localTransform.ValueRO.Position;
                // TODO: this is obviously not working as it should
                currentPosition.y = yOffset;
                localTransform.ValueRW.Position = currentPosition;
            }
        }
    }
}