using Data;
using Unity.Entities;

namespace Manager {
    public interface IAnimationManager : IManager {
        void SetAnimationState(Entity entity, AnimationState newAnimationState);
    }
}