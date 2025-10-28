using BuildingLogic;
using Data;
using Unity.Entities;

namespace Manager {
    public interface IJobManager : IManager {

        /// <summary>
        /// Increases the amount of pending jobs on an entity. (e.g. a building)
        /// This call enable/disable Tags on the entity. NEVER use pendingJobs-field directly
        /// </summary>
        /// <param name="resourceEntity"></param>
        /// <param name="em"></param>
        /// <param name="ecb"></param>
        void PendingJobsIncrease(Entity resourceEntity, ref EntityManager em, EntityCommandBuffer? ecb = null);

        /// <summary>
        /// Decreases the amount of pending jobs on an entity. (e.g. a building)
        /// This call enable/disable Tags on the entity. NEVER use pendingJobs-field directly
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="em"></param>
        /// <param name="ecb"></param>
        /// <returns></returns>
        bool PendingJobsDecrease(Entity entity, ref EntityManager em, EntityCommandBuffer? ecb = null);
        
        /// <summary>
        /// Create a construciton job
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        Entity CreateConstructionJob(Entity owner);

        /// <summary>
        /// Create generic job
        /// </summary>
        /// <param name="owner">The entity that is emitting the job (usually a building)</param>
        /// <param name="jobTarget">The entity that where to job is going to happen. Uses its localTransform as job-position</param>
        /// <param name="jobType">The logic that behind that job. This also controls if the settler needs to go back to the owner after finishing the job or not</param>
        /// <param name="duration">The work duration</param>
        /// <param name="ecb">optionally an EntityCommandBuffer to defere entity-actions</param>
        /// <returns></returns>
        Entity CreateGenericJob(Entity owner, Entity jobTarget, JobType jobType, float duration, EntityCommandBuffer? ecb = null);
        IJobLogic GetJobLogic(JobType jobType);
    }
}