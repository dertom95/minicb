using Components;
using Unity.Entities;

namespace Systems {
    public partial struct HouseEntityStateSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            HandleEntityStateChange(ref state);
        }

        /// <summary>
        /// Handle changes on the state (Created,Destroyed, etc)
        /// </summary>
        /// <param name="state"></param>
        private void HandleEntityStateChange(ref SystemState state) {
            foreach (var (houseComponent, entityStateComponent, entity) in SystemAPI.Query<HouseComponent, EntityStateComponent>().WithEntityAccess()) {
                switch (entityStateComponent.currentState) {
                    case EntityStateComponent.EntityStateType.Created: {
                        // house just finished building
                        Mgr.settlerManager.IncreaseMaxSettlerAmount(houseComponent.increaseSettlerAmount);
                        break;
                    }
                    case EntityStateComponent.EntityStateType.Destroyed: {
                        EntityCommandBuffer ecb = EntityUtils.CreateEntityCommandBufferEndSim();
                            // house just finishing being destroyed (and the entity can soon be destroyed completely)
                        Mgr.settlerManager.DecreaseMaxSettlerAmount(houseComponent.increaseSettlerAmount);
                        break;
                    }
                    default:
                        // nothing to do
                        break;
                }
            }
        }

        private EntityCommandBuffer GetECB(ref SystemState state) {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb;
        }
    }
}