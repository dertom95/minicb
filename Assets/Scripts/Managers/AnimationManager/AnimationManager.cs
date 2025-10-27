using Components;
using Data;
using Unity.Entities;
using UnityEngine.Assertions;

namespace Manager {
    public class AnimationManager : IManager, IAnimationManager {

        public AnimationManager() { }

        EntityManager entityManager;

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public void Dispose() {
        }

        public void SetAnimationState(Entity entity, AnimationState newAnimationState) {
            Assert.IsTrue(entityManager.HasComponent<AnimationStateComponent>(entity));
            AnimationStateComponent animationStateComp = entityManager.GetComponentData<AnimationStateComponent>(entity);

            if (animationStateComp.animationState == newAnimationState) {
                // nothing to do
                return;
            }

            animationStateComp.animationState = newAnimationState;
            entityManager.SetComponentData(entity, animationStateComp);

            if (entityManager.HasComponent<JumpAnimationComponent>(entity)) {
                JumpAnimationComponent jumpAnimationComp = entityManager.GetComponentData<JumpAnimationComponent>(entity);
                jumpAnimationComp.time = 0;

                switch (newAnimationState) {
                    case AnimationState.idle: jumpAnimationComp.currentSpeed = 0; break;
                    case AnimationState.walking: jumpAnimationComp.currentSpeed = jumpAnimationComp.walkSpeed; break;
                    case AnimationState.working: jumpAnimationComp.currentSpeed = jumpAnimationComp.workSpeed; break;
                    default:
                        jumpAnimationComp.currentSpeed = 0;
                        Assert.IsFalse(true, "JumpAnimation: unknown animationstate");
                        break;
                }
                entityManager.SetComponentData(entity, jumpAnimationComp);
            }
        }
    }
}