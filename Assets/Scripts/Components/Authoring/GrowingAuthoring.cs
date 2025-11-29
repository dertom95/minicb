using Components;
using Unity.Entities;
using UnityEngine;

public class GrowAuthoring : MonoBehaviour {
    public float initialAge = 0;
    public float growAmountPerSecond = 0.1f;

    public class Baker : Baker<GrowAuthoring> {
        public override void Bake(GrowAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new AgeComponent {
                age = authoring.initialAge,
            });
            AddComponent(entity, new GrowingComponent {
                growTimePerSecond = authoring.growAmountPerSecond
            });
        }
    }
}