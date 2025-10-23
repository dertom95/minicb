using Data;
using Unity.Entities;

namespace Components {
    /// <summary>
    /// Entity that can emit jobs
    /// </summary>
    public struct JobEmitterComponent : IComponentData {
        /// <summary>
        /// The job type
        /// </summary>
        public JobType jobType;

        /// <summary>
        /// The max amount of current jobs (running and pending)
        /// </summary>
        public byte maxJobs;

        /// <summary>
        /// The current amount of running and pending jobs spawned by this building
        /// </summary>
        public byte currentJobAmount;

        /// <summary>
        /// The amount of seconds to work for a job to finish
        /// </summary>
        public byte jobDurationInSeconds;
    }
}