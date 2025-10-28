using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace Manager {
    public interface IPhysicsManager : IManager {

        /// <summary>
        /// Get world's physicsworld-singleton
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public PhysicsWorldSingleton GetPhysicsWorldSingleton(World world);

        /// <summary>
        /// Get world's physicsWorld
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public PhysicsWorld GetPhysicsWorld(World world);

        /// <summary>
        /// Try to find one entity in the specified sphere. 
        /// Optionally add functionFilter to query specific entity-properties
        /// </summary>
        /// <param name="world"></param>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="layerMask"></param>
        /// <param name="funcFilter"></param>
        /// <returns></returns>
        Entity FindFirstEntityInRadius(World world, float3 position, float radius, uint layerMask, Func<EntityManager, Entity, bool> funcFilter = null);
        
        /// <summary>
        /// Try to pickray in the defaultworld with default collisionFilter
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        bool TryToPickRaycast(out Unity.Physics.RaycastHit hit);

        /// <summary>
        /// Try to pickray in the defaultworld
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        bool TryToPickRaycast(out Unity.Physics.RaycastHit result, CollisionFilter collisionFilter);

        /// <summary>
        /// Try to pickray in the specified world, with specified-camera (usually Camera.main) and a specific collisionFilter (usually CollisionFilter.default)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="camera"></param>
        /// <param name="collisionFilter"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryToPickRaycast(World world, Camera camera, CollisionFilter collisionFilter, out Unity.Physics.RaycastHit result);
    }
}