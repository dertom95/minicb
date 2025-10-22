using Components;
using PlasticGui;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Assertions;

public class JobSpawnEntity : JobLogicBase {
    private Random random = new Random(1);


    public override bool NeedsToGoBackToOwner() {
        return false;    
    }

    public override bool CanAcceptJob(Entity jobEntity, EntityManager em, ref JobComponent job) {
        bool baseResult = base.CanAcceptJob(jobEntity, em, ref job);

        // TODO: Check if there is enough space
        bool foundSpace = true;

        return foundSpace && baseResult;
    }

    public override bool OnStarted(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        bool success = base.OnStarted(ref ecb, ref jobEntity, ref state, ref job, dt);
        if (!success) {
            return false;
        }

        EntityManager em = state.EntityManager;

        LocalTransform localTransform = em.GetComponentData<LocalTransform>(job.jobOwner);
        FieldOfInfluenceComponent foiComp = em.GetComponentData<FieldOfInfluenceComponent>(job.jobOwner);
        // TODO: Use some logic to find (potentially) valid positions
        float3 randomPosition = Utils.RandomPointInRadiusXZ(localTransform.Position, foiComp.radius, minRadius: 5.5f);
        // TODO: check for valid position
        bool checkSuccessful = true;
        
        if (!checkSuccessful) {
            return false;
        }
        job.jobPosition = randomPosition;
        // TODO: move jobEntity to randomPosition and give it the physics in order to keep the room free!
        job.jobTarget = jobEntity; 
        
        DynamicBuffer<SpawnEntityPrefabBufferElement> spawnPrefabs = em.GetBuffer<SpawnEntityPrefabBufferElement>(job.jobOwner);
        SpawnEntityComponent spawnEntityComponent = em.GetComponentData<SpawnEntityComponent>(job.jobOwner);
        byte currentSpawnIdx = spawnEntityComponent.lastSelectedIdx;
        switch (spawnEntityComponent.mode) {
            case SpawnEntityComponent.Mode.Random: // TODO: With this logic the first idx will be always 0
                currentSpawnIdx = (byte)random.NextInt(0, spawnPrefabs.Length);
                break;
            case SpawnEntityComponent.Mode.RoundRobin:
                spawnEntityComponent.lastSelectedIdx = (byte)((spawnEntityComponent.lastSelectedIdx + 1) % spawnPrefabs.Length);
                ecb.SetComponent(job.jobOwner, spawnEntityComponent);
                break;
            default:
                Assert.IsTrue(false, "Unknown spawnEntity-Mode");
                break;
        }

        Entity spawnPrefabEntity = spawnPrefabs[currentSpawnIdx].prefabEntity;
        job.jobGenericEntity = spawnPrefabEntity; // store the prefab

        ecb.SetComponent(jobEntity, job);

        return true;
    }


    public override void OnFinishedWorking(ref EntityCommandBuffer ecb, ref Entity jobEntity, ref SystemState state, ref JobComponent job, float dt) {
        base.OnFinishedWorking(ref ecb, ref jobEntity, ref state, ref job, dt);

        Assert.AreNotEqual(Entity.Null, job.jobGenericEntity);

        Entity spawnedEntity = ecb.Instantiate(job.jobGenericEntity);

        ecb.SetComponent(spawnedEntity, new LocalTransform {
            Position = job.jobPosition,
            Rotation = Utils.GetRandomYRotation(),
            Scale = Utils.random.NextFloat(0.85f, 1.0f)
        });
    }
}