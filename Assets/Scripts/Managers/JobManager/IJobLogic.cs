using Components;
using Data;
using Manager;
using Unity.Entities;
using Unity.Transforms;

namespace BuildingLogic {
    public interface IJobLogic {
        /// <summary>
        /// If the job is finished working, does the settler need to return to the owner? Or is finished after working
        /// </summary>
        /// <returns></returns>
        bool NeedsToGoBackToOwner();

        /// <summary>
        /// Are all requirements ok to run the job?
        /// </summary>
        /// <returns></returns>
        bool CanAcceptJob(Entity jobEntity, EntityManager em, ref JobComponent job);
        
        /// <summary>
        /// A settler got assigned to this job
        /// </summary>
        void OnStarted(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt);

        /// <summary>
        /// The settler reached the target where it is starting its work
        /// </summary>
        void OnReachedTarget(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt);

        /// <summary>
        /// The settler is working right now
        /// </summary>
        void OnWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt, float progress);

        /// <summary>
        /// The settler finished working
        /// </summary>
        void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt);

        /// <summary>
        /// The settler went to back to the owner-entity (optional, if NeedsToGoBackToOwner returns true)
        /// </summary>
        void OnReachedOwner(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt);

        /// <summary>
        /// The job is finished
        /// </summary>
        void OnExit(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt);
    }
}