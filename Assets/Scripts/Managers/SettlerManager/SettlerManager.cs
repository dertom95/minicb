using Components;
using Components.Tags;
using Data;
using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Assertions;

namespace Manager {
    /// <summary>
    /// Settler-Specific functions
    /// </summary>
    public class SettlerManager : ISettlerManager {
        public event EventHandler EventSettlerAmountChanged;

        private EntityManager entityManager;
        private IAssetManager assetManager;
        private IDataManager dataManager;
        private Entity playerSingletonEntity;
        private EntityQuery endsimulationSingletonQuery;

        public SettlerManager() {
        }

        public void Init() {
            UnityEngine.Debug.Log("SettlerManager");
            dataManager = Mgr.dataManager; //TODO: check if is this doing something at all in comparision to using Mgr.dataManager every time?
            assetManager = Mgr.assetManager;

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery playerSingletonQuery = entityManager.CreateEntityQuery(typeof(MainPlayerSingleton));
            playerSingletonEntity = playerSingletonQuery.GetSingletonEntity();
            
            endsimulationSingletonQuery = entityManager.CreateEntityQuery(typeof(EndSimulationEntityCommandBufferSystem.Singleton));
        }

        public void Dispose() {
        }

        private SettlerAmountComponent GetSettlerAmountComponent() {
            SettlerAmountComponent settlerAmountComponent = entityManager.GetComponentData<SettlerAmountComponent>(playerSingletonEntity);
            return settlerAmountComponent;
        }

        public int GetCurrentSettlerAmount() {
            SettlerAmountComponent settlerAmountComponent = GetSettlerAmountComponent();
            return settlerAmountComponent.currentSettlerAmount;
        }

        public int GetMaxSettlerAmount() {
            SettlerAmountComponent settlerAmountComponent = GetSettlerAmountComponent();
            return settlerAmountComponent.maxSettlerAmount;
        }

        public int IncreaseCurrentSettlerAmount(int amount) {
            SettlerAmountComponent settlerAmountComponent = GetSettlerAmountComponent();
            settlerAmountComponent.currentSettlerAmount += amount;
            Assert.IsTrue(settlerAmountComponent.currentSettlerAmount <= settlerAmountComponent.maxSettlerAmount);
            
            entityManager.SetComponentData(playerSingletonEntity, settlerAmountComponent);
            EventSettlerAmountChanged.Invoke(this,null);
            return settlerAmountComponent.currentSettlerAmount;
        }

        public int DecreaseCurrentSettlerAmount(int amount) {
            SettlerAmountComponent settlerAmountComponent = GetSettlerAmountComponent();
            settlerAmountComponent.currentSettlerAmount -= amount;
            Assert.IsTrue(settlerAmountComponent.currentSettlerAmount >= 0);

            entityManager.SetComponentData(playerSingletonEntity, settlerAmountComponent);
            EventSettlerAmountChanged.Invoke(this, null);
            return settlerAmountComponent.currentSettlerAmount;
        }

        public int IncreaseMaxSettlerAmount(int amount) {
            SettlerAmountComponent settlerAmountComponent = GetSettlerAmountComponent();
            settlerAmountComponent.maxSettlerAmount += amount;

            entityManager.SetComponentData(playerSingletonEntity, settlerAmountComponent);
            EventSettlerAmountChanged.Invoke(this, null);
            return settlerAmountComponent.maxSettlerAmount;
        }

        public int DecreaseMaxSettlerAmount(int amount) {
            SettlerAmountComponent settlerAmountComponent = GetSettlerAmountComponent();
            settlerAmountComponent.maxSettlerAmount -= amount;
            Assert.IsTrue(settlerAmountComponent.maxSettlerAmount >= 0);

            entityManager.SetComponentData(playerSingletonEntity, settlerAmountComponent);
            EventSettlerAmountChanged.Invoke(this, null);
            return settlerAmountComponent.maxSettlerAmount;
        }

        public Entity SpawnSettler(SettlerType settlerType, float3 position, float scale, float3 rotation) {
            Entity entityPrefab = assetManager.GetSettlerEntityPrefab(settlerType);
            Assert.IsFalse(entityPrefab == Entity.Null);
            Entity newEntity = entityManager.Instantiate(entityPrefab);

            LocalTransform newTransform = new LocalTransform {
                Position = position,
                Rotation = quaternion.EulerXYZ(new float3(math.radians(rotation.x), math.radians(rotation.y), math.radians(rotation.z))),
                Scale = scale
            };
            entityManager.SetComponentData(newEntity, newTransform);

            return newEntity;
        }
    }
}