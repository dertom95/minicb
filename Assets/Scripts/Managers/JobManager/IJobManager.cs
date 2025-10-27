using BuildingLogic;
using Data;
using Unity.Entities;

namespace Manager {
    public interface IJobManager : IManager {
        bool PendingJobsDecrease(Entity entity, ref EntityManager em, EntityCommandBuffer? ecb = null);
        void PendingJobsIncrease(Entity resourceEntity, ref EntityManager em, EntityCommandBuffer? ecb = null);
        Entity CreateConstructionJob(Entity owner);
        Entity CreateGenericJob(Entity owner, Entity jobTarget, JobType jobType, float duration, EntityCommandBuffer? ecb = null);
        IJobLogic GetJobLogic(JobType jobType);
    }
}