using Unity.Entities;

namespace Components {
    /// <summary>
    /// Handles a worktimer
    /// </summary>
    public struct WorkTimerComponent : IComponentData, IEnableableComponent {
        /// <summary>
        /// the runtime value of the current work-timer
        /// </summary>
        public float currentTime;

        /// <summary>
        /// The total time for a work to be finished
        /// </summary>
        public float workTimeTotal; 
    }
}