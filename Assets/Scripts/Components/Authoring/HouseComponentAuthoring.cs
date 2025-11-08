using Components;
using Unity.Entities;
using UnityEngine;

public class HouseComponentAuthoring : MonoBehaviour {
    /// <summary>
    /// Increase the player's max settler amount
    /// </summary>
    [Tooltip("Increase the player's max settler amount")]
    public byte increaseSettlerAmount;

    public class Baker : Baker<HouseComponentAuthoring> {
        public override void Bake(HouseComponentAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new HouseComponent { 
                increaseSettlerAmount = authoring.increaseSettlerAmount
            });
        }
    }
}