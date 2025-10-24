using Components;
using Data;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class JobEmitterComponentAuthoring : MonoBehaviour {
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

    public class Baker : Baker<JobEmitterComponentAuthoring> {
        public override void Bake(JobEmitterComponentAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new JobEmitterComponent { 
                currentJobAmount = 0,
                jobDurationInSeconds = authoring.jobDurationInSeconds,
                jobType = authoring.jobType,
                maxJobs = authoring.maxJobs
            });
        }
    }
}