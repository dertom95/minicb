using Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ChildAuthoring : MonoBehaviour {
    public GameObject parentReference;

    public class ChildBaker : Baker<ChildAuthoring> {
        public override void Bake(ChildAuthoring authoring) {
            // Get the entity for the child GameObject (this one)
            Entity childEntity = GetEntity(TransformUsageFlags.Dynamic);  

            // Get the entity for the parent GameObject (from the reference)
            Entity parentEntity = GetEntity(authoring.parentReference, TransformUsageFlags.None);

            // Add the Parent component to the child entity
            AddComponent(childEntity, new ParentRelationComponent { parentEntity = parentEntity });
        }
    }
}