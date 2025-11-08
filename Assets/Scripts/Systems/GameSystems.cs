using Components;
using Systems.SystemGroups;
using Unity.Entities;

namespace Systems {
    [UpdateInGroup(typeof(GameSystemGroup),OrderFirst = true)]
    public partial struct GameSystem : ISystem {
        private Entity gameEntity;
        private Entity mainPlayerEntity;

        public void OnCreate(ref SystemState state) {
            UnityEngine.Debug.Log("GAMESYSTEM");
            gameEntity = state.EntityManager.CreateEntity(typeof(GameSingletonComponent));
            mainPlayerEntity = state.EntityManager.CreateEntity(typeof(PlayerComponent), typeof(SettlerAmountComponent), typeof(MainPlayerSingleton));
        }
    }
}