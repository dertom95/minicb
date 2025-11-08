using Components;
using Systems.SystemGroups;
using Unity.Entities;
using Unity.Transforms;
using static Components.EntityStateComponent;

namespace Systems {
    /// <summary>
    /// This system simply disables the EntityStateComponent at the end of the SimulationLifeCycle, at this point every other system should have done what it has to do using the state-change
    /// </summary>
    [UpdateInGroup(typeof(GameSystemGroup),OrderLast = true)]
    public partial struct EntityStateChangeDisableSystem : ISystem {
        

        public void OnCreate(ref SystemState state) {
        }

        public void OnUpdate(ref SystemState state) {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            EntityManager em = state.EntityManager;

            foreach (var (entityStateComponent, entity) 
                     in SystemAPI.Query<EntityStateComponent>()
                                    .WithEntityAccess()) 
            {
                ecb.SetComponentEnabled<EntityStateComponent>(entity,false);                        
            }
        }


    }
}