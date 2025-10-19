using Data;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PrefabListAuthoring : MonoBehaviour {
    [System.Serializable]
    public struct BuildingPrefabEntry {
        public BuildingType type;
        public GameObject prefab;
    }

    public List<BuildingPrefabEntry> buildingPrefabs;

    class Baker : Baker<PrefabListAuthoring> {
        public override void Bake(PrefabListAuthoring authoring) {
            // Create a singleton entity to hold the buffer
            var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<BuildingPrefabBufferElement>(entity);

            foreach (var entry in authoring.buildingPrefabs) {
                if (entry.prefab != null) {
                    var prefabEntity = GetEntity(entry.prefab, TransformUsageFlags.Dynamic);
                    buffer.Add(new BuildingPrefabBufferElement {
                        type = entry.type,
                        prefabEntity = prefabEntity
                    });
                }
            }
        }
    }

}