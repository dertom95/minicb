using Components;
using Data;
using Manager;
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Manager {
    public interface ISettlerManager  : IManager{
        event EventHandler EventSettlerAmountChanged;

        int GetCurrentSettlerAmount();
        int GetMaxSettlerAmount();

        int IncreaseCurrentSettlerAmount(int amount);
        int DecreaseCurrentSettlerAmount(int amount);
        
        int IncreaseMaxSettlerAmount(int amount);
        int DecreaseMaxSettlerAmount(int amount);

        Entity SpawnSettler(SettlerType settlerType, float3 position, float scale, float3 rotation);
    }
}