using Components;
using Data;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class BuildingComponentAuthoring : MonoBehaviour
{
    /// <summary>
    /// Buidling Type of this Entity
    /// </summary>
    [Tooltip("Buidling Type of this Entity")]
    public BuildingType buildingType;

    /// <summary>
    /// Building Costs
    /// </summary>
    [Tooltip("Building Costs")]
    public BuildingCosts buildingCosts;

    public class Baker : Baker<BuildingComponentAuthoring> {
        public override void Bake(BuildingComponentAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BuildingComponent { 
                buildingType = authoring.buildingType,
                buildingCosts = authoring.buildingCosts,
                currentState = BuildingState.UnderConstruction,
            });

            AddComponent(entity, new EntityStateComponent { 
                currentState = EntityStateComponent.EntityStateType.Created
            });
            SetComponentEnabled<EntityStateComponent>(entity, false);
        }
    }
}
