using Components;
using Data;
using Unity.Entities;
using UnityEngine;

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

    /// <summary>
    /// Max amount of Jobs for this building to spawn at the same time
    /// </summary>
    [Tooltip("Max amount of Jobs for this building to spawn at the same time")]
    public byte maxJobs;

    /// <summary>
    /// JobType for spawned jobs
    /// </summary>
    [Tooltip("JobType for spawned jobs")]
    public JobType jobType;

    /// <summary>
    /// Job Duration for spawned jobs
    /// </summary>
    [Tooltip("Job Duration for spawned jobs")]
    public byte jobDurationInSeconds;

    public class Baker : Baker<BuildingComponentAuthoring> {
        public override void Bake(BuildingComponentAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BuildingComponent { 
                buildingType = authoring.buildingType,
                buildingCosts = authoring.buildingCosts,
                currentState = BuildingState.UnderConstruction,
                maxJobs = authoring.maxJobs,
                jobType = authoring.jobType,
                jobDurationInSeconds = authoring.jobDurationInSeconds,
            });
        }
    }
}
