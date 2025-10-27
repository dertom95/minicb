using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace Manager {
    public interface IPhysicsManager : IManager, IManagerUpdateable {
        Entity FindFirstEntityInRadius(World world, float3 position, float radius, uint layerMask, Func<EntityManager, Entity, bool> funcFilter = null);
        void SetWorld(World world, Camera camera);
        bool TryToPickRaycast(out Unity.Physics.RaycastHit hit);
        bool TryToPickRaycast(out Unity.Physics.RaycastHit result, CollisionFilter collisionFilter);
    }
}