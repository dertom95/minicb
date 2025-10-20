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
    public override JobType JobType => JobType.Construction;

    public override bool NeedsToGoBackToOwner() {
        return false;
    }

    public override void OnWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt, float progress) {
        EntityManager em = state.EntityManager;
        LocalTransform settlerTransform = em.GetComponentData<LocalTransform>(job.jobOwner);
        settlerTransform.Scale = math.max(BuildingManager.BUILDING_CONSTRUCTIONSITE_SCALE, progress);
        ecb.SetComponent(job.jobOwner, settlerTransform);
    }

    public override void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        ecb.AddComponent<TagBuilt>(job.jobOwner);
        ecb.SetComponentEnabled<TagUnderConstruction>(job.jobOwner,false);
        ecb.AddComponent<TagWorking>(job.jobOwner);
    }

}
