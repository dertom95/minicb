using Components;
using Data;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class FieldOfInfluenceAuthoring : MonoBehaviour
{
    public float radius;

    public class Baker : Baker<FieldOfInfluenceAuthoring> {
        public override void Bake(FieldOfInfluenceAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new FieldOfInfluenceComponent { 
                radius = authoring.radius
            });
        }
    }
}
