namespace Manager {
    using NUnit.Framework;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using Unity.Physics;

    public class InputManager : IManager {

        EntityManager entityManager;
        EntityQuery physicsWorldSingletonQuery;

        public InputManager() {
            Init();
        }

        public void Init() {
            var world = World.DefaultGameObjectInjectionWorld;
            entityManager = world.EntityManager;

            physicsWorldSingletonQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
        }

        public void Update(float dt) {
            InstantiateTreeOnLeftClick();
        }

        private void InstantiateTreeOnLeftClick() {
            if (Input.GetMouseButtonDown(0)) {
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
                    Entity entityPrefab = DataManager.Instance.GetBuildingEntityPrefab(Data.BuildingType.gatherer);
                    Entity newEntity = entityManager.Instantiate(entityPrefab);
                    LocalTransform newTransform = new LocalTransform {
                        Position = hit.Position,
                        Rotation = quaternion.EulerXYZ(new float3(0f, math.radians(45f), 0f)),
                        Scale = 1f
                    };
                    entityManager.SetComponentData(newEntity, newTransform);

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

