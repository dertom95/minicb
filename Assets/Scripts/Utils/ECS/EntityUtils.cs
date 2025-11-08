
using Components;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Assertions;
using UnityEngine.InputSystem.HID;
using static Components.EntityStateComponent;

public static class EntityUtils {

    private static EntityQuery queryEndSimulationCommandBufferSingleton;

    public static void InitQueries(ref SystemState state) {
        var entityQueryBuilder = new global::Unity.Entities.EntityQueryBuilder(global::Unity.Collections.Allocator.Temp);
        queryEndSimulationCommandBufferSingleton =
            entityQueryBuilder
                .WithAll<global::Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton>()
                .WithOptions(global::Unity.Entities.EntityQueryOptions.IncludeSystems)
                .Build(ref state);
        entityQueryBuilder.Reset();
        entityQueryBuilder.Dispose();
    }

    public static EntityCommandBuffer CreateEntityCommandBufferEndSim() {
        EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = queryEndSimulationCommandBufferSingleton.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(World.DefaultGameObjectInjectionWorld.Unmanaged);
        return ecb;
    }

    /// <summary>
    /// Returns a specific child from that entity
    /// Cautious: The children-id start with 0 (not with 1, where the children entitis start in the buffer
    /// </summary>
    /// <param name="em"></param>
    /// <param name="entity"></param>
    /// <param name="childId"></param>
    /// <returns></returns>
    public static Entity GetChild(EntityManager em, Entity entity, int childId = 0) {
        Assert.IsTrue(em.HasComponent<LinkedEntityGroup>(entity)); 

        DynamicBuffer<LinkedEntityGroup> linkedEntityGroup = em.GetBuffer<LinkedEntityGroup>(entity);
        Assert.IsTrue(linkedEntityGroup.Length >= childId);

        return linkedEntityGroup[childId + 1].Value;
    }

    /// <summary>
    /// Set materialID for an entity using MaterialMeshInfo.
    /// Caution: Not sure how stable that is.
    /// </summary>
    /// <param name="em"></param>
    /// <param name="entity"></param>
    /// <param name="materialID"></param>
    public static void SetEntityMaterialID(EntityManager em, Entity entity, int materialID) {
        var meshInfo = em.GetComponentData<MaterialMeshInfo>(entity);
        meshInfo.Material = materialID;
        UnityEngine.Debug.Log("Set MaterialID:" + materialID);
        em.SetComponentData(entity, meshInfo);
    }

    /// <summary>
    /// Set materialID for an entity using MaterialMeshInfo.
    /// Caution: Not sure how stable that is.
    /// </summary>
    /// <param name="em"></param>
    /// <param name="entity"></param>
    /// <param name="materialID"></param>
    public static int GetEntityMaterialID(EntityManager em, Entity entity) {
        var meshInfo = em.GetComponentData<MaterialMeshInfo>(entity);
        return meshInfo.Material;
    }

    public static float GetPhysicsSphereWorldRadius(EntityManager em, Entity entity) {
        PhysicsCollider physicsCollider = em.GetComponentData<PhysicsCollider>(entity);
        BlobAssetReference<Collider> colliderBlob = physicsCollider.Value;
        Assert.AreEqual(ColliderType.Sphere, colliderBlob.Value.Type);

        LocalToWorld localToWorld = em.GetComponentData<LocalToWorld>(entity);
        float3x3 rotationScaleMatrix = new float3x3(localToWorld.Value);

        // Each column represents the scaled axis in world space
        float3 scale;
        scale.x = math.length(rotationScaleMatrix.c0);
        scale.y = math.length(rotationScaleMatrix.c1);
        scale.z = math.length(rotationScaleMatrix.c2);

        if (colliderBlob.Value.Type == ColliderType.Sphere) {
            unsafe {
                // Cast to SphereCollider
                var sphereCollider = (SphereCollider*)colliderBlob.GetUnsafePtr();

                // Access the sphere geometry
                SphereGeometry sphereGeometry = sphereCollider->Geometry;

                // Get the radius
                float radius = sphereGeometry.Radius;
                float maxScale = math.cmax(scale); // Gets the largest component
                float worldRadius = radius * maxScale;

                return worldRadius;
            }
        } else {
            throw new System.Exception("Unsupported PhysicsType:"+ colliderBlob.Value.Type);
        }
    }

    public static void SetEntityState(this EntityManager em, Entity entity, EntityStateType entityStateType) {
        EntityStateComponent entityStateComponent = em.GetComponentData<EntityStateComponent>(entity);
        entityStateComponent.currentState = entityStateType;
        em.SetComponentData(entity, entityStateComponent);
        em.SetComponentEnabled<EntityStateComponent>(entity, true);
    }

    /// <summary>
    /// Check if the specific entity is a House.
    /// </summary>
    /// <param name="em"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBuilding(this EntityManager em, Entity entity) {
        bool isHouse = em.HasComponent<BuildingComponent>(entity);
        return isHouse;
    }


    /// <summary>
    /// Check if the specific entity is a House.
    /// </summary>
    /// <param name="em"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsHouse(this EntityManager em, Entity entity) {
        Assert.IsTrue(em.IsBuilding(entity));
        bool isHouse = em.HasComponent<HouseComponent>(entity);
        return isHouse;
    }
}