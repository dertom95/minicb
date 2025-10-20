using Components;
using Data;
using Manager;
using Unity.Entities;
using Unity.Transforms;

namespace BuildingLogic {
    public interface IJobLogic {
        JobType JobType { get; }
        bool NeedsToGoBackToOwner();
        void OnReachedTarget(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt);
        void OnWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt, float progress);
        void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt);
        void OnReachedOwner(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt);
    }
}