namespace Manager {
    using Components;
    using Data;
    using System;
    using System.Collections.Generic;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class InputManager : IManager, IManagerUpdateable {
        public enum InputMode {
            SpawnBuilding
        }

        public event EventHandler<BuildingType> EventSelectedBuildingChanged;

        private BuildingType currentBuilding = BuildingType.woodcutter;

        private static readonly InputManager inputManager = new InputManager();

        public static InputManager Instance => inputManager;

        EntityManager entityManager;
        EntityQuery physicsWorldSingletonQuery;

        private InputManager() {
        }


        public void Init() {
            var world = World.DefaultGameObjectInjectionWorld;
            entityManager = world.EntityManager;

            physicsWorldSingletonQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            SetCurrentBuilding(BuildingType.gatherer);
        }

        public void Dispose() {
        }

        public void Update(float dt) {
            HandleKeyboard();
            InstantiateBuildingOnLeftClick();
        }

        private void HandleKeyboard() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                currentBuilding = BuildingType.gatherer;
                Debug.Log("Selected gatherer");
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                currentBuilding = BuildingType.woodcutter;
                Debug.Log("Selected woodcutter");
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                currentBuilding = BuildingType.mason;
                Debug.Log("Selected mason");
            } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
                currentBuilding = BuildingType.woodworker;
                Debug.Log("Selected woodworker");
            }
        }

        private bool IsMouseButtonDown(int btn, bool ignoreButtonIfOverUI = true) {
            return Input.GetMouseButtonDown(btn) && (!ignoreButtonIfOverUI || !UIManager.Instance.IsMouseOverUI());
        }

        private void InstantiateBuildingOnLeftClick() {

            if (IsMouseButtonDown(0)) {

                PhysicsWorldSingleton physicsWorld = physicsWorldSingletonQuery.GetSingleton<PhysicsWorldSingleton>();

                var world = World.DefaultGameObjectInjectionWorld;
                var physicsWorldSystem = physicsWorldSingletonQuery.GetSingleton<PhysicsWorldSingleton>();
                var collisionWorld = physicsWorldSystem.CollisionWorld;

                UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //// Define your ray
                float3 rayOrigin = ray.origin;
                float3 rayDirection = ray.direction;
                float rayLength = 10000f;

                var rayInput = new RaycastInput {
                    Start = rayOrigin,
                    End = rayOrigin + rayDirection * rayLength,
                    Filter = CollisionFilter.Default
                };


                if (collisionWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit)) {
                    Debug.Log($"Hit: Entity:{hit.Entity}  Pos:{hit.Position} ColKey:{hit.ColliderKey}");

                    bool hadEnoughResourceToBuildBuilding = DataManager.Instance.TryToRemoveBuildingCostsFromInventory(currentBuilding);
                    if (!hadEnoughResourceToBuildBuilding) {
                        UnityEngine.Debug.Log("Not enough resources to build:" + currentBuilding);
                        return;
                    }
                    Entity newEntity = BuildingManager.Instance.SpawnBuilding(currentBuilding, hit.Position);

                    if (newEntity == Entity.Null) {
                        // couldn't build
                        return;
                    }

                    //Entity entityPrefab = DataManager.Instance.GetBuildingEntityPrefab(Data.BuildingType.gatherer);
                    //Entity newEntity = entityManager.Instantiate(entityPrefab);
                    //LocalTransform newTransform = new LocalTransform {
                    //    Position = hit.Position,
                    //    Rotation = quaternion.EulerXYZ(new float3(0f, math.radians(45f), 0f)),
                    //    Scale = 1f
                    //};
                    //entityManager.SetComponentData(newEntity, newTransform);

                    //EntityPrefabs entityPrefabs = prefabSingletonQuery.GetSingleton<EntityPrefabs>();
                    //Assert.IsTrue(entityPrefabs.treePrefab != Entity.Null);
                    //Entity islandEntity = islandSingletonQuery.GetSingletonEntity();
                    //Assert.IsTrue(islandEntity != Entity.Null);

                    //Entity newEntity = entityManager.Instantiate(entityPrefabs.treePrefab);
                    //LocalTransform newTransform = new LocalTransform {
                    //    Position = hit.Position,
                    //    Rotation = quaternion.EulerXYZ(new float3(0f, math.radians(45f), 0f)),
                    //    Scale = 1f
                    //};
                    //entityManager.SetComponentData(newEntity, newTransform);
                    //entityManager.AddComponentData(newEntity, new Parent { Value = islandEntity });
                }

            }
        }

        //public bool MouseOverUIElement() {
        //    bool mouseOver = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        //    Debug.Log("MouseOver UI:" + mouseOver);
        //    return mouseOver;
        //}

        public void SetCurrentBuilding(BuildingType buildingType) {
            this.currentBuilding = buildingType;

            EventSelectedBuildingChanged?.Invoke(this, buildingType);
        }

        //private void RotateIsland() {
        //    InputComponent inputComponent = new InputComponent();
        //    if (Input.GetKey(KeyCode.A)) {
        //        inputComponent.rotate += -1;
        //    }
        //    if (Input.GetKey(KeyCode.D)) {
        //        inputComponent.rotate += 1;
        //    }

        //    Entity inputEntity = inputSingletonQuery.GetSingletonEntity();
        //    entityManager.SetComponentData(inputEntity, inputComponent);
        //}
    }
}

