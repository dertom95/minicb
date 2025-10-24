using Data;
using Unity.Entities;

namespace Components {
    /// <summary>
    /// Handles amount of incoming jobs. Only enabled if jobs are pending
    /// </summary>
    public struct JobTargetComponent : IComponentData, IEnableableComponent {
        /// <summary>
        /// DO NOT CHANGE MANUALLY! Use JobManager.PendingJobsIncrease(..) or JobManager.PendingJobsDecrease(..)
        /// 
        /// The amount of jobs that are still depending on the this entity
        /// </summary>
        public byte pendingJobs;
    }
}