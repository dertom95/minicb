using BuildingLogic;
using Components;
using Components.Tags;
using Data;
using Manager;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;

public class JobResourceToResource : JobLogicBase {
    IDataManager dataManager = Mgr.dataManager;

    public override bool NeedsToGoBackToOwner() {
        return false;
    }

    public override bool CanAcceptJob(Entity jobEntity, EntityManager em, ref JobComponent job) {
        Assert.IsTrue(em.HasBuffer<RTRInputResource>(job.jobOwner));
        Assert.IsTrue(em.HasBuffer<RTROutputResource>(job.jobOwner));

        DynamicBuffer<RTRInputResource> inputResources = em.GetBuffer<RTRInputResource>(job.jobOwner);

        
        // check if the inputResources are available in the inventory
        foreach (RTRInputResource resource in inputResources) {
            bool enoughResourceInInventory = dataManager.HasResourceInGlobalInventory(resource.resIn.resourceType, resource.resIn.resourceAmount);
            if (!enoughResourceInInventory) {
                // not all resources available for conversion
                return false;
            }
        }

        DynamicBuffer<RTROutputResource> outputResources = em.GetBuffer<RTROutputResource>(job.jobOwner);
        foreach (RTROutputResource resource in outputResources) {
            bool limitReached = dataManager.IsLimitReached(resource.resOut.resourceType);
            if (limitReached) {
                // at least one output resource would hit the limit
                return false;
            }
        }
        return true;
    }

    public override bool OnStarted(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        bool success = base.OnStarted(ref ecb, ref jobEntity, ref state, ref job, dt);
        if (!success) {
            return false;
        }

        EntityManager em = state.EntityManager;
        bool requirementSuccess = CanAcceptJob(jobEntity, em, ref job);
        if (!requirementSuccess) {
            return false;
        }

        DynamicBuffer<RTRInputResource> inputResources = em.GetBuffer<RTRInputResource>(job.jobOwner);

        // check if the inputResources are available in the inventory
        foreach (RTRInputResource resource in inputResources) {
            success = dataManager.RemoveResFromGlobalInventory(resource.resIn);
            Assert.IsTrue(success, "ResourceRemoval should be guaranteed by CanAcceptJob(..). Only possible way for this to fail is using multithreading!?");
        }
        return true;
    }

    public override void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        base.OnFinishedWorking(ref ecb, ref jobEntity, ref state, ref job, dt);

        EntityManager em = state.EntityManager;
        DynamicBuffer<RTROutputResource> outputResources = em.GetBuffer<RTROutputResource>(job.jobOwner);

        // check if the inputResources are available in the inventory
        foreach (RTROutputResource resource in outputResources) {
            dataManager.AddToGlobalInventory(resource.resOut);
        }
    }
}
