using BuildingLogic;
using Components;
using Components.Tags;
using Data;
using Manager;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;

public class JobResourceToResource : JobLogicBase {
    public override bool NeedsToGoBackToOwner() {
        return false;
    }

    public override bool CanAcceptJob(Entity jobEntity, EntityManager em, ref JobComponent job) {
        Assert.IsTrue(em.HasBuffer<RTRInputResource>(job.jobOwner));
        Assert.IsTrue(em.HasBuffer<RTROutputResource>(job.jobOwner));

        DynamicBuffer<RTRInputResource> inputResources = em.GetBuffer<RTRInputResource>(job.jobOwner);

        DataManager dm = DataManager.Instance;
        
        // check if the inputResources are available in the inventory
        foreach (RTRInputResource resource in inputResources) {
            bool enoughResourceInInventory = dm.HasResInGlobalInventory(resource.resIn.resourceType, resource.resIn.resourceAmount);
            if (!enoughResourceInInventory) {
                // not all resources available for conversion
                return false;
            }
        }

        DynamicBuffer<RTROutputResource> outputResources = em.GetBuffer<RTROutputResource>(job.jobOwner);
        foreach (RTROutputResource resource in outputResources) {
            bool limitReached = dm.IsLimitReached(resource.resOut.resourceType);
            if (limitReached) {
                // at least one output resource would hit the limit
                return false;
            }
        }
        return true;
    }

    public override void OnStarted(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        EntityManager em = state.EntityManager;
        Assert.IsTrue(CanAcceptJob(jobEntity, em, ref job));

        DynamicBuffer<RTRInputResource> inputResources = em.GetBuffer<RTRInputResource>(job.jobOwner);

        DataManager dm = DataManager.Instance;

        // check if the inputResources are available in the inventory
        foreach (RTRInputResource resource in inputResources) {
            bool success = dm.RemoveResFromGlobalInventory(resource.resIn);
            if (!success) {
                Debug.LogError("Couldn't remove all resources for R2R-Job!");
            }
        }
    }

    public override void OnWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt, float progress) {
    }

    public override void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        EntityManager em = state.EntityManager;
        DynamicBuffer<RTROutputResource> outputResources = em.GetBuffer<RTROutputResource>(job.jobOwner);

        DataManager dm = DataManager.Instance;

        // check if the inputResources are available in the inventory
        foreach (RTROutputResource resource in outputResources) {
            dm.AddToGlobalInventory(resource.resOut);
        }
    }
}
