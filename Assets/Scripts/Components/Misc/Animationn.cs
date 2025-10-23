using Data;
using Unity.Entities;

namespace Components {
    /// <summary>
    /// Handles a bouncy-animation
    /// </summary>
    //TODO: I really do not like the size of this component for the amount of information! But for the time being,... maybe create a byte-based float?
    public struct JumpAnimationComponent : IComponentData, IEnableableComponent {
        public float currentSpeed;
        public float height;
        public float time;

        public float walkSpeed;
        public float workSpeed;
    }

    /// <summary>
    /// Entity's AnimationState
    /// </summary>
    public struct AnimationStateComponent : IComponentData {
        public AnimationState animationState;
    }
}