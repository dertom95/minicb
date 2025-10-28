using Components;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class SpawnEntityComponentAuthoring : MonoBehaviour {
    /// <summary>
    /// The mode this entity spawner operates
    /// </summary>    
    [Tooltip("The mode this entity spawner operates")]
    public SpawnEntityComponent.Mode mode;

    /// <summary>
    /// The prefabs this spawner should spawn
    /// </summary>  
    [Tooltip("The prefabs this spawner should spawn")]
    public List<GameObject> spawnPrefabs = new List<GameObject>();

    public class Baker : Baker<SpawnEntityComponentAuthoring> {
        public override void Bake(SpawnEntityComponentAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SpawnEntityComponent { 
                mode = authoring.mode
            });

            DynamicBuffer<SpawnEntityPrefabBufferElement> prefabBuffer = AddBuffer<SpawnEntityPrefabBufferElement>(entity);

            foreach (GameObject prefab in authoring.spawnPrefabs) {
                Entity prefabEntity = GetEntity(prefab, TransformUsageFlags.None);
                prefabBuffer.Add(new SpawnEntityPrefabBufferElement { 
                    prefabEntity = prefabEntity,
                });
            }
        }
    }
}