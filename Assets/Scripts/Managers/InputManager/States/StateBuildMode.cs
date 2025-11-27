using Data;
using Manager;
using System;
using System.Diagnostics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using static Manager.InputManager;


namespace Manager {
    /// <summary>
    /// Building is selected and preview is attached to cursor, waiting for left-click to set or right-click to reset
    /// </summary>
    public class StateBuildMode : IState<InputManagerContext>, IUpdatableState<InputManagerContext> {
        private EntityManager entityManager;
        private BuildingType currentBuildingType;
        public BuildingType BuildingType => currentBuildingType;
        private IPhysicsManager physicsManager;
        private IInputManager inputManager;

        bool isColliding = false;
        private int originalMaterialId = 0;
        private float sphereRadius = 0;

        private Entity previewEntity;
        private Entity visualEntity;
        private float3 currentPosition;

        private bool notEnoughResourcesError = false;


        public Entity PreviewEntity => previewEntity;

        public void OnStateEnter(InputManagerContext ctx,object input) {
            currentBuildingType = (BuildingType)input;
            
            entityManager = ctx.world.EntityManager;
            physicsManager = ctx.physicsManager;
            inputManager = Mgr.inputManager;

            previewEntity = Mgr.buildingManager.SpawnPreviewBuilding(currentBuildingType,removePhysicsOnToplevelEntity: false);
            visualEntity = EntityUtils.GetChild(entityManager, previewEntity);
            Assert.IsTrue(visualEntity != Entity.Null);

            originalMaterialId = EntityUtils.GetEntityMaterialID(entityManager, visualEntity);

            sphereRadius = EntityUtils.GetPhysicsSphereWorldRadius(entityManager, previewEntity);
            entityManager.RemoveComponent<PhysicsCollider>(previewEntity);

            UpdatePreviewEntity(ctx.world);
        }

        public bool OnUpdate(ref InputManagerContext ctx) {
            UnityEngine.Debug.Log("BUILDMODE-UPDATE");
            UpdatePreviewEntity(ctx.world);
            bool keepRunning = HandleInput();
            return keepRunning;
        }

        private void UpdatePreviewEntity(World world) {
            CollisionFilter filter = CollisionFilter.Default;
            filter.CollidesWith = 1 << Config.LAYER_TERRAIN;

            if (physicsManager.TryToPickRaycast(out Unity.Physics.RaycastHit hit, filter)) {
                UnityEngine.Debug.Log("Hit:" + hit.Entity);
                currentPosition = hit.Position;

                entityManager.SetComponentData(previewEntity, LocalTransform.FromPosition(hit.Position));

                Entity entity = Mgr.physicsManager.FindFirstEntityInRadius(world, hit.Position, sphereRadius, (1u << Config.LAYER_RESOURCE) | (1u << Config.LAYER_BUILDINGS));
                isColliding = entity != Entity.Null;
                if (isColliding) {
                    // TODO: don't spam data write
                    EntityUtils.SetEntityMaterialID(entityManager, visualEntity, Config.MAT_PALETTE_ERROR);
                    UnityEngine.Debug.Log($"Collision: {entity}");
                } else {
                    // TODO: don't spam data write
                    UnityEngine.Debug.Log($"NO Collision: {entity}");
                    EntityUtils.SetEntityMaterialID(entityManager, visualEntity, originalMaterialId);
                }
            }       
        }

        private void TryToBuildBuilding() {
            if (isColliding) {
                UnityEngine.Debug.Log("Cannot build, due to collision!");
            }

            bool hadEnoughResourceToBuildBuilding = Mgr.dataManager.TryToRemoveBuildingCostsFromInventory(currentBuildingType);
            if (!hadEnoughResourceToBuildBuilding) {
                UnityEngine.Debug.Log("Not enough resources to build:" + currentBuildingType);
                notEnoughResourcesError = true;
                return;
            }
            notEnoughResourcesError = false;

            Entity newEntity = Mgr.buildingManager.SpawnBuilding(currentBuildingType, currentPosition);
            Assert.IsTrue(newEntity != Entity.Null);
        }

        private bool HandleInput() {
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape)) {
                return false;
            }

            if (!isColliding && inputManager.IsMouseButtonDown(0)) {
                TryToBuildBuilding();
            }
            if (notEnoughResourcesError) { 
                if (UnityEngine.Input.GetMouseButton(0)) {
                    // TODO: don't spam data write
                    EntityUtils.SetEntityMaterialID(entityManager, visualEntity, Config.MAT_PALETTE_ERROR);
                } else {
                    // TODO: don't spam data write
                    EntityUtils.SetEntityMaterialID(entityManager, visualEntity, originalMaterialId);
                    notEnoughResourcesError = false;
                }
            }   

            return true;
        }

        public void OnStateExit() {
            if (previewEntity != Entity.Null) {
                entityManager.DestroyEntity(previewEntity);
                previewEntity = Entity.Null;
            }
        }

        public static void SetEntityMaterial(EntityManager entityManager, Entity entity, int materialIndex) {
            if (entityManager.HasComponent<MaterialMeshInfo>(entity)) {
                var meshInfo = entityManager.GetComponentData<MaterialMeshInfo>(entity);
                meshInfo.Material = (ushort)materialIndex;
                entityManager.SetComponentData(entity, meshInfo);
            }
        }
    }

}

