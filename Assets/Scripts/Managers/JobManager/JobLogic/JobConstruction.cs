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

    public override void OnWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt, float progress) {
        base.OnWorking(ref ecb, ref jobEntity, ref state, ref job, dt, progress);

        EntityManager em = state.EntityManager;
        LocalTransform settlerTransform = em.GetComponentData<LocalTransform>(job.jobOwner);
        settlerTransform.Scale = math.lerp(BuildingManager.BUILDING_CONSTRUCTIONSITE_SCALE, 1.0f, progress);
        ecb.SetComponent(job.jobOwner, settlerTransform);
    }

    public override void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        base.OnFinishedWorking(ref ecb, ref jobEntity, ref state, ref job, dt);

        ecb.RemoveComponent<TagUnderConstruction>(job.jobOwner);
        ecb.AddComponent<TagBuilt>(job.jobOwner);
        ecb.AddComponent<TagWorking>(job.jobOwner);
        ecb.AddComponent<TagCreateJobs>(job.jobOwner);
    }

}
