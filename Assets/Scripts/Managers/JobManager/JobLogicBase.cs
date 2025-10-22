using BuildingLogic;
using Components;
using Data;
using Unity.Entities;
using UnityEngine;

public abstract class JobLogicBase : IJobLogic {

    /// <summary>
    /// If the job is finished working, does the settler need to return to the owner? Or is finished after working
    /// </summary>
    /// <returns></returns>
    public abstract bool NeedsToGoBackToOwner();

    /// <summary>
    /// Are all requirements ok to run the job?
    /// </summary>
    /// <returns></returns>
    public virtual bool CanAcceptJob(Entity jobEntity, EntityManager em, ref JobComponent job) {
        return true;
    }

    /// <summary>
    /// A settler got assigned to this job
    /// </summary>
    public virtual bool OnStarted(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        return true;
    }

    /// <summary>
    /// The settler reached the target where it is starting its work
    /// </summary>
    public virtual void OnReachedTarget(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
    }

    /// <summary>
    /// The settler is working right now
    /// </summary>
    public virtual void OnWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt, float progress) {
    }

    /// <summary>
    /// The settler finished working
    /// </summary>
    public virtual void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
    }

    /// <summary>
    /// The settler went to back to the owner-entity (optional, if NeedsToGoBackToOwner returns true)
    /// </summary>
    public virtual void OnReachedOwner(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
    }

    /// <summary>
    /// The job is finished
    /// </summary>
    public virtual void OnExit(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
    }
}
