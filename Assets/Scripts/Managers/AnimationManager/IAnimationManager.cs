using Data;
using Unity.Entities;

namespace Manager {
    public interface IAnimationManager : IManager {

        /// <summary>
        /// Set the animationState for a specific entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="newAnimationState"></param>
        void SetAnimationState(Entity entity, AnimationState newAnimationState);
    }
}