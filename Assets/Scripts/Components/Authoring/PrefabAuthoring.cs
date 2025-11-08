using Components;
using Data;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class PrefabListAuthoring : MonoBehaviour {
    [System.Serializable]
    public struct BuildingPrefabEntry {
        public BuildingType type;
        public GameObject prefab;
    }

    [System.Serializable]
    public struct ResourcePrefabEntry {
        public ResourcePrefabType type;
        public GameObject prefab;
    }

    [System.Serializable]
    public struct SettlerPrefabEntity {
        public GameObject prefab;
    }

    public List<BuildingPrefabEntry> buildingPrefabs;
    public List<ResourcePrefabEntry> resourcePrefabs;
    public List<SettlerPrefabEntity> settlerPrefabs;

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

            var resBuffer = AddBuffer<ResourcePrefabBufferElement>(entity);
            foreach (var entry in authoring.resourcePrefabs) {
                if (entry.prefab != null) {
                    var prefabEntity = GetEntity(entry.prefab, TransformUsageFlags.Dynamic);
                    resBuffer.Add(new ResourcePrefabBufferElement {
                        type = entry.type,
                        prefabEntity = prefabEntity
                    });
                }
            }

            var settlerBuffer = AddBuffer<SettlerPrefabBufferElement>(entity);
            foreach (var entry in authoring.settlerPrefabs) {
                if (entry.prefab != null) {
                    var prefabEntity = GetEntity(entry.prefab, TransformUsageFlags.Dynamic);
                    settlerBuffer.Add(new SettlerPrefabBufferElement {
                        prefabEntity = prefabEntity
                    });
                }
            }
        }
    }

}