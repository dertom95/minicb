using Components;
using Data;
using Unity.Entities;
using UnityEngine;

public class BuildingComponentAuthoring : MonoBehaviour
{
    public BuildingType buildingType;
    public BuildingCosts buildingCosts;
    public byte maxJobs;

    public class Baker : Baker<BuildingComponentAuthoring> {
        public override void Bake(BuildingComponentAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BuildingComponent { 
                buildingType = authoring.buildingType,
                buildingCosts = authoring.buildingCosts,
                currentState = BuildingState.UnderConstruction,
                maxJobs = authoring.maxJobs
            });
        }
    }
}
