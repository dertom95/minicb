using Components;
using Data;
using Unity.Entities;
using UnityEngine;

public class ResourceToResourceAuthoring : MonoBehaviour
{
    /// <summary>
    /// Input Resources to be used for Resource2Resource-conversion
    /// </summary>
    [Tooltip("Input Resources to be used for Resource2Resource-conversion")]
    public ResourceAmount[] inputResources = new ResourceAmount[0];
    /// <summary>
    /// Output Resources that are the result of the conversion
    /// </summary>
    [Tooltip("The resource type this entity is looking for in its FieldOfInfluence")]
    public ResourceAmount[] outputResources = new ResourceAmount[0];
   
    public class Baker : Baker<ResourceToResourceAuthoring> {
        public override void Bake(ResourceToResourceAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ResourceToResourceComponent { 
            });
            DynamicBuffer<RTRInputResource> inputBuffer = AddBuffer<RTRInputResource>(entity);
            DynamicBuffer<RTROutputResource> outputBuffer = AddBuffer<RTROutputResource>(entity);
            foreach (var inRes in authoring.inputResources) {
                inputBuffer.Add(new RTRInputResource() { resIn=inRes });
            }
            foreach (var outRes in authoring.outputResources) {
                outputBuffer.Add(new RTROutputResource() { resOut = outRes });
            }
        }
    }
}
