using BuildingLogic;
using Components;
using Components.Tags;
using Data;
using Manager;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class JobConstruction : JobLogicBase {

    public override bool NeedsToGoBackToOwner() {
        return false;
    }

    public override bool OnStarted(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        bool success = base.OnStarted(ref ecb, ref jobEntity, ref state, ref job, dt);
        if (!success) {
            return false;
        }
        state.EntityManager.SetEntityState(job.jobOwner, EntityStateComponent.EntityStateType.OnCreation);
        return true;
    }

    public override void OnWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt, float progress) {
        base.OnWorking(ref ecb, ref jobEntity, ref state, ref job, dt, progress);

        EntityManager em = state.EntityManager;
        LocalTransform buildingTransform = em.GetComponentData<LocalTransform>(job.jobOwner);
        buildingTransform.Scale = math.lerp(BuildingManager.BUILDING_CONSTRUCTIONSITE_SCALE, 1.0f, progress);
        ecb.SetComponent(job.jobOwner, buildingTransform);
    }

    public override void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        base.OnFinishedWorking(ref ecb, ref jobEntity, ref state, ref job, dt);

        ecb.RemoveComponent<TagUnderConstruction>(job.jobOwner);
        ecb.AddComponent<TagBuilt>(job.jobOwner);
        ecb.AddComponent<TagWorking>(job.jobOwner);
        ecb.AddComponent<TagCreateJobs>(job.jobOwner);
        // change entity state
        state.EntityManager.SetEntityState(job.jobOwner, EntityStateComponent.EntityStateType.Created);
    }

}
