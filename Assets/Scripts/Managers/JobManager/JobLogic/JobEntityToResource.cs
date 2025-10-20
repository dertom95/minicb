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
        EntityManager em = state.EntityManager;
        ResourceComponent resComp = em.GetComponentData<ResourceComponent>(job.jobTarget);

        Assert.IsFalse(em.HasComponent<InventoryComponent>(job.jobSettler),"Job-Worker already has InventoryComponent!");

        InventoryComponent invComp = new InventoryComponent();
        // TODO: Use a more generic approach
        switch (resComp.resourceType) {
            case ResourceType.Stone: invComp.stone = resComp.resourceAmountPerIteration; break;
            case ResourceType.Food: invComp.food = resComp.resourceAmountPerIteration; break;
            case ResourceType.Wood: invComp.wood = resComp.resourceAmountPerIteration; break;
            default: Assert.IsTrue(false,"Unknown resourceType"); break;
        }
        ecb.AddComponent(job.jobSettler, invComp);
    }

    public override void OnWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt, float progress) {
    }

    public override void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        ResourceComponent resComp = state.EntityManager.GetComponentData<ResourceComponent>(job.jobTarget);
        resComp.pendingJobs--;
        ecb.SetComponent(job.jobTarget, resComp);

        bool destroyEntity = resComp.pendingJobs == 0 && resComp.iterationsLeft == 0;
        if (destroyEntity) {
            ecb.DestroyEntity(job.jobTarget);
        }
    }

    public override void OnReachedOwner(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        // back in our work-building
        EntityManager em = state.EntityManager;
        Assert.IsTrue(em.HasComponent<InventoryComponent>(job.jobSettler),"Settler finished 'EntityToResource'-Job, but got no InventoryComponent");
        InventoryComponent invComp = em.GetComponentData<InventoryComponent>(job.jobSettler);
        InventoryComponent globalInvComp = DataManager.Instance.AddToGlobalInventory(invComp);

        Debug.Log($"Collected: Wood:{invComp.wood} Stone:{invComp.stone} Food:{invComp.food}!");
        Debug.Log($"=> New Global Inventory: Wood:{globalInvComp.wood} Stone:{globalInvComp.stone} Food:{globalInvComp.food}!");
        ecb.RemoveComponent<InventoryComponent>(job.jobSettler);
        // TODO add inventory to global inventory
    }
}
