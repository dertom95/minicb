using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems {
    public partial struct SettlerBirthdSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            // TOOD: Restrict this to only run, when it is needed (Tag on Game/Player-Singleton)
            int currentAmountSettlers = Mgr.settlerManager.GetCurrentSettlerAmount();
            int maxAmountSettlers = Mgr.settlerManager.GetMaxSettlerAmount();
            int neededSettlers = maxAmountSettlers - currentAmountSettlers;
            if (neededSettlers == 0) {
                return;
            }

            for (int i = 0; i < neededSettlers; i++) {
                float3 spawnPos = Utils.RandomPointInRadiusXZ(float3.zero, 5, 1);
                Entity newSettler = Mgr.settlerManager.SpawnSettler(Data.SettlerType.Worker, spawnPos, 1, default);
                Mgr.settlerManager.IncreaseCurrentSettlerAmount(1);
            }
        }
    }
}