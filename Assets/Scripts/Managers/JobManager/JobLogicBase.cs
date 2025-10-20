using BuildingLogic;
using Components;
using Data;
using Unity.Entities;
using UnityEngine;

public abstract class JobLogicBase : IJobLogic {
    public abstract JobType JobType { get; }

    public abstract bool NeedsToGoBackToOwner();

    public virtual void OnReachedTarget(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
    }

    public virtual void OnWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt, float progress) {
    }

    public virtual void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
    }

    public virtual void OnReachedOwner(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
    }

}
