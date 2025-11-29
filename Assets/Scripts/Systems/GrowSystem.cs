using Components;
using Unity.Entities;
using Unity.Transforms;

namespace Systems {
    public partial struct GrowSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BuildingPrefabBufferElement>();
        }

        public void OnUpdate(ref SystemState state) {
            float dt = SystemAPI.Time.DeltaTime;

            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ( var (growingComponent,ageComponent, localTransform,entity) 
                        in SystemAPI.Query<GrowingComponent, RefRW<AgeComponent>, RefRW<LocalTransform>>().WithEntityAccess()
            ) {
                float newAge = ageComponent.ValueRO.age + growingComponent.growTimePerSecond * dt;
                if (newAge > 1.0f) {
                    newAge = 1.0f;
                    ecb.SetComponentEnabled<GrowingComponent>(entity, false);
                }
                ageComponent.ValueRW.age = newAge;

                float newScale = newAge;
                localTransform.ValueRW.Scale = newScale;
            }
        }
    }
}