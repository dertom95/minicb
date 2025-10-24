using Components;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class IterationsComponentAuthoring : MonoBehaviour {
    public byte amountIterations;
    public bool destroyEntityOnNoIterationsLeft;

    public class Baker : Baker<IterationsComponentAuthoring> {
        public override void Bake(IterationsComponentAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new IterationsComponent {
                iterationsLeft = authoring.amountIterations
            });

            if (authoring.destroyEntityOnNoIterationsLeft) {
                AddComponent<TagIterationAutoDestroy>(entity);
            }
        }
    }

}