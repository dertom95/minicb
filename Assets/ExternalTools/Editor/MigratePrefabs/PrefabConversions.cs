using Components;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class PrefabConversions : PrefabMigrationHelper {

    //[MenuItem("Minicb/Migration/SplitBuildingComp_to_JobEmitter")]
    //public static void MigratePrefabs() {
    //    MigratePrefabFrom<BuildingComponentAuthoring>((prefab, buildingComponent) => {
    //        // Check if JobEmitterComponent already exists
    //        var jobEmitter = prefab.GetComponent<JobEmitterComponentAuthoring>();
    //        if (jobEmitter != null) {
    //            // don't overwrite if already present
    //            return false;
    //        }

    //        jobEmitter = prefab.AddComponent<JobEmitterComponentAuthoring>();

    //        // Transfer data example (replace with your actual fields)
    //        jobEmitter.jobType = buildingComponent.jobType;
    //        jobEmitter.jobDurationInSeconds = buildingComponent.jobDurationInSeconds;
    //        jobEmitter.maxJobs = buildingComponent.maxJobs;

    //        return true;
    //    });
    
    //}
}

