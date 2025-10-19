using Data;
using Unity.Entities;

namespace Components {
    public struct SettlerComponent : IComponentData {
        /// <summary>
        /// The reference to the current job this Settlers is working on. (Entity.null if idle)
        /// </summary>
        public Entity currentJob;

        /// <summary>
        /// The walk speed
        /// </summary>
        public float walkSpeed;

        /// <summary>
        /// The work speed. 
        /// </summary>
        public float workSpeed;

        /// <summary>
        /// SettlerType
        /// </summary>
        public SettlerType settlerType;

        /// <summary>
        /// A bitmask of jobs this settler can accept
        /// </summary>
        public JobType acceptJobs;

        /// <summary>
        /// Current settlerstate
        /// </summary>
        // TODO: Is this state needed at all as I maintain this state as Tag? :thinking:
        public SettlerState settlerState;
    }
}