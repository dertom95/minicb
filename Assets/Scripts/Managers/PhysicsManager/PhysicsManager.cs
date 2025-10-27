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

        private EntityQuery physicsWorldSingletonQuery;
        private PhysicsWorldSingleton physicsWorldSingleton;
        private EntityManager entityManager;
        private Camera camera;
        private CollisionWorld collisionWorld;
        private PhysicsWorld physicsWorld;

        public void Init() {
            SetWorld(World.DefaultGameObjectInjectionWorld, Camera.main);
        }

        public void Dispose() {
        }

        public void Update(float dt) {
            UpdatePhysicsSingleton();
        }

        public void SetWorld(World world, Camera camera) {
            this.world = world;
            this.camera = camera;
            entityManager = world.EntityManager;

            physicsWorldSingletonQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            UpdatePhysicsSingleton();
        }

        private void UpdatePhysicsSingleton() {
            physicsWorldSingleton = physicsWorldSingletonQuery.GetSingleton<PhysicsWorldSingleton>();
            physicsWorld = physicsWorldSingleton.PhysicsWorld;
            collisionWorld = physicsWorldSingleton.CollisionWorld;
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

            if (physicsWorldSingleton.OverlapSphere(position, radius, ref resultHits, filter)) {
                for (int i = 0; i < resultHits.Length; i++) {
                    var hit = resultHits[i];
                    var entity = physicsWorldSingleton.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
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

        public bool TryToPickRaycast(out Unity.Physics.RaycastHit hit) {
            return TryToPickRaycast(out hit, CollisionFilter.Default);
        }

        public bool TryToPickRaycast(out Unity.Physics.RaycastHit result, CollisionFilter collisionFilter) {
            result = default;

            //physicsWorldSingleton = physicsWorldSingletonQuery.GetSingleton<PhysicsWorldSingleton>();
            collisionWorld = physicsWorldSingleton.CollisionWorld;
            physicsWorld = physicsWorldSingleton.PhysicsWorld;

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
