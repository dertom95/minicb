using Components;
using Data;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

public static class PhysicsUtils {
    /// <summary>
    /// Finds the first entity with a collider in the specified physics layer within radius of position.
    /// Returns Entity.Null if none found.
    /// </summary>
    /// <param name="world">The ECS World to get PhysicsWorld from</param>
    /// <param name="position">Center position to search around</param>
    /// <param name="radius">Search radius</param>
    /// <param name="resourceLayer">Physics layer bit index used by resource entities</param>
    /// <returns>Entity found or Entity.Null if none</returns>

    public static Entity FindFirstEntityInRadius(World world, float3 position, float radius, UInt32 layerMask, Func<EntityManager,Entity,bool> funcFilter=null) {
        EntityManager entityManager = world.EntityManager;
        EntityQuery physicsWorldSingletonQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

        PhysicsWorldSingleton physicsWorld = physicsWorldSingletonQuery.GetSingleton<PhysicsWorldSingleton>();
        
        NativeList<DistanceHit> resultHits = new NativeList<DistanceHit>(Allocator.Temp);

        CollisionFilter filter = new CollisionFilter {
            BelongsTo = ~0u,               // Belongs to all layers
            CollidesWith = layerMask,        // Collides only with layer 3
            GroupIndex = 0
        };

        if (physicsWorld.OverlapSphere(position, radius, ref resultHits, filter)) {
            for (int i = 0; i < resultHits.Length; i++) {
                var hit = resultHits[i];
                var entity = physicsWorld.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                if (funcFilter != null) {
                    if (funcFilter(entityManager, entity)) {
                        return entity;
                    }
                    continue;
                }
                return entity;
            }
        }
        return Entity.Null;
    }

}
