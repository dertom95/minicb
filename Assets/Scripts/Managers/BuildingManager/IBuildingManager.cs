using Data;
using Unity.Entities;
using Unity.Mathematics;

namespace Manager {
    public interface IBuildingManager : IManager {
        /// <summary>
        /// Spawn building at position
        /// </summary>
        /// <param name="buildingType"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        Entity SpawnBuilding(BuildingType buildingType, float3 position, bool immediateBuild=false, bool createConstructionJobs = true);

        /// <summary>
        /// Spawn building at position,scale and rotation
        /// </summary>
        /// <param name="buildingType"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="immediateBuild"></param>
        /// <returns></returns>
        Entity SpawnBuilding(BuildingType buildingType, float3 position, float scale, float3 rotation, bool immediateBuild = false, bool createConstructionJobs=true);

        /// <summary>
        /// Spawns a preview building that got decorated components stripped away
        /// </summary>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        Entity SpawnPreviewBuilding(BuildingType buildingType, bool removePhysicsOnToplevelEntity=true);
    }
}