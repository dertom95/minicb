using Components;
using Data;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(IterationsComponentAuthoring), typeof(JobTargetComponentAuthoring))]
public class ResourceComponentAuthoring : MonoBehaviour
{
    /// <summary>
    /// The resource type of this entity
    /// </summary>
    [Tooltip("The resource type of this entity")]
    public ResourceType resourceType;

    /// <summary>
    /// The amount of resources added to the inventory per iteration
    /// </summary>
    [Tooltip("The amount of resources added to the inventory per iteration")]
    public byte resourceAmountPerIteration;

    public class Baker : Baker<ResourceComponentAuthoring> {
        public override void Bake(ResourceComponentAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ResourceComponent { 
                resourceType = authoring.resourceType,
                resourceAmountPerIteration = authoring.resourceAmountPerIteration
            });
            
        }
    }
}
