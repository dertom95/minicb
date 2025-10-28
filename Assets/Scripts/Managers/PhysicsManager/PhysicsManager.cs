namespace Manager {
    using Manager;
    using System;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Systems;
    using UnityEngine;

    public class PhysicsManager : IPhysicsManager {
        private World world;

        public void Init() {
        }

        public void Dispose() {
        }

        public PhysicsWorldSingleton GetPhysicsWorldSingleton(World world) {
            EntityManager em = world.EntityManager;
            EntityQuery query = em.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = query.GetSingleton<PhysicsWorldSingleton>();
            return physicsWorldSingleton;
        }

        public PhysicsWorld GetPhysicsWorld(World world) {
            var physicsWorldSingleton = GetPhysicsWorldSingleton(world);
            return physicsWorldSingleton.PhysicsWorld;
        }

        /// <summary>
        /// Finds the first entity with a collider in the specified physics layer within radius of position.
        /// Returns Entity.Null if none found.
        /// </summary>
        /// <param name="world">The ECS World to get PhysicsWorld from</param>
        /// <param name="position">Center position to search around</param>
        /// <param name="radius">Search radius</param>
        /// <param name="resourceLayer">Physics layer bit index used by resource entities</param>
        /// <returns>Entity found or Entity.Null if none</returns>
        public Entity FindFirstEntityInRadius(World world, float3 position, float radius, UInt32 layerMask, Func<EntityManager, Entity, bool> funcFilter = null) {
            NativeList<DistanceHit> resultHits = new NativeList<DistanceHit>(Allocator.Temp);

            CollisionFilter filter = new CollisionFilter {
                BelongsTo = ~0u,               // Belongs to all layers
                CollidesWith = layerMask,        // Collides only with layer 3
                GroupIndex = 0
            };

            var physicsWorldSingleton = GetPhysicsWorldSingleton(world);

            if (physicsWorldSingleton.OverlapSphere(position, radius, ref resultHits, filter)) {
                for (int i = 0; i < resultHits.Length; i++) {
                    var hit = resultHits[i];
                    var entity = physicsWorldSingleton.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                    if (funcFilter != null) {
                        if (funcFilter(world.EntityManager, entity)) {
                            return entity;
                        }
                        continue;
                    }
                    return entity;
                }
            }
            return Entity.Null;
        }

        public bool TryToPickRaycast(out Unity.Physics.RaycastHit hit) {
            return TryToPickRaycast(out hit, CollisionFilter.Default);
        }

        public bool TryToPickRaycast(out Unity.Physics.RaycastHit result, CollisionFilter collisionFilter) {
            return TryToPickRaycast(World.DefaultGameObjectInjectionWorld, Camera.main, collisionFilter, out result);
        }

        public bool TryToPickRaycast(World world, Camera camera, CollisionFilter collisionFilter, out Unity.Physics.RaycastHit result) {
            result = default;

            //physicsWorldSingleton = physicsWorldSingletonQuery.GetSingleton<PhysicsWorldSingleton>();
            var physicsWorld = GetPhysicsWorld(world);

            UnityEngine.Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            //// Define your ray
            float3 rayOrigin = ray.origin;
            float3 rayDirection = ray.direction;
            float rayLength = 10000f;

            var rayInput = new RaycastInput {
                Start = rayOrigin,
                End = rayOrigin + rayDirection * rayLength,
                Filter = collisionFilter,
            };


            if (physicsWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit)) {
                result = hit;
                return true;
            }
            return false;
        }
    }
}
