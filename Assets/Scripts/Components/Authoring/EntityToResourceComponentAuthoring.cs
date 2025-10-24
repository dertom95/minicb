using Components;
using Data;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class EntityToResourceComponentAuthoring : MonoBehaviour
{
    /// <summary>
    /// The resource type this entity is looking for in its FieldOfInfluence
    /// </summary>
    [Tooltip("The resource type this entity is looking for in its FieldOfInfluence")]
    public ResourceType searchResourceType;

    public class Baker : Baker<EntityToResourceComponentAuthoring> {
        public override void Bake(EntityToResourceComponentAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EntityToResourceComponent { 
                searchResourceType = authoring.searchResourceType
            });
        }
    }
}
