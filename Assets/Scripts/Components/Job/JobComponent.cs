using Data;
using Unity.Entities;
using Unity.Mathematics;

namespace Components {
    /// <summary>
    /// General Job-Data
    /// </summary>
    public struct JobComponent : IComponentData {
        /// <summary>
        /// The building that created this job
        /// </summary>
        public Entity jobOwner;

        /// <summary>
        /// The settler that is doing the job
        /// </summary>
        public Entity jobSettler;

        /// <summary>
        /// The target entity on which the job is executed
        /// </summary>
        public Entity jobTarget;

        /// <summary>
        /// The position where the job is located
        /// </summary>
        public float3 jobPosition;

        /// <summary>
        /// Time this job needs to finish
        /// </summary>
        public float jobDuration;
    
        /// <summary>
        /// The job type
        /// </summary>
        public JobType jobType;

        /// <summary>
        /// Current job state
        /// </summary>
        public JobState jobState;
    }
}