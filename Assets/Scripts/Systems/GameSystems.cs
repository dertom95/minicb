using Components;
using Manager;
using Systems.SystemGroups;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems {
    [UpdateInGroup(typeof(GameSystemGroup),OrderFirst = true)]
    [UpdateAfter(typeof(PrefabLoaderSystem))]
    public partial struct GameSystem : ISystem {
        private Entity gameEntity;
        private Entity mainPlayerEntity;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BuildingPrefabBufferElement>();

            UnityEngine.Debug.Log("GAMESYSTEM");
            gameEntity = state.EntityManager.CreateEntity(typeof(GameSingletonComponent));
            state.EntityManager.AddComponent<UIComponent>(gameEntity);

            mainPlayerEntity = state.EntityManager.CreateEntity(typeof(PlayerComponent), typeof(SettlerAmountComponent), typeof(MainPlayerSingleton));
        }

        public void OnUpdate(ref SystemState state) {
            state.Enabled = false;
            CreateSettlers();
        }

        private void CreateSettlers() {
            for (int i=0, iEnd = Config.Instance.InitialSettlerAmount; i < iEnd; i++) {
                float3 randomPosition = Utils.RandomPointInRadiusXZ(float3.zero, 10, 2);
                Entity settler = Mgr.settlerManager.SpawnSettler(Data.SettlerType.Worker, randomPosition, 1, float3.zero);
            }
        }
    }
}