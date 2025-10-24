using BuildingLogic;
using Components;
using Components.Tags;
using Data;
using Manager;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;

public class JobEntityToResource : JobLogicBase {
    public override bool NeedsToGoBackToOwner() {
        return true;
    }

    public override void OnReachedTarget(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        base.OnReachedTarget(ref ecb, ref jobEntity, ref state, ref job, dt);

        EntityManager em = state.EntityManager;
        ResourceComponent resComp = em.GetComponentData<ResourceComponent>(job.jobTarget);

        DynamicBuffer<ResourceBufferElement> inventoryBuffer = em.GetBuffer<ResourceBufferElement>(job.jobSettler);
        Assert.AreEqual(0, inventoryBuffer.Length);

        ecb.AppendToBuffer(job.jobSettler, new ResourceBufferElement {
            amount = resComp.resourceAmountPerIteration,
            type = resComp.resourceType
        });

    }

    public override void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        base.OnFinishedWorking(ref ecb, ref jobEntity, ref state, ref job, dt);

        EntityManager em = state.EntityManager;

        JobManager.PendingJobsDecrease(job.jobTarget, ref em, ecb);
    }



    public override void OnReachedOwner(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        base.OnReachedOwner(ref ecb, ref jobEntity, ref state, ref job, dt);

        // back in our work-building
        EntityManager em = state.EntityManager;
        Assert.IsTrue(em.HasComponent<ResourceBufferElement>(job.jobSettler),"Settler finished 'EntityToResource'-Job, but got no InventoryComponent");
        
        DynamicBuffer<ResourceBufferElement> inventoryBuffer = em.GetBuffer<ResourceBufferElement>(job.jobSettler);

        foreach (ResourceBufferElement resElem in inventoryBuffer) {
            int totalAmount = DataManager.Instance.AddToGlobalInventory(resElem.type, resElem.amount);
            Debug.Log($"Collected:{resElem.type} : {resElem.amount} => Total:{totalAmount}");
        }
        inventoryBuffer.Clear();
        // TODO add inventory to global inventory
    }
}
