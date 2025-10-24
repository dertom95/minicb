using Components;
using Data;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class JobTargetComponentAuthoring : MonoBehaviour
{
    public class Baker : Baker<JobTargetComponentAuthoring> {
        public override void Bake(JobTargetComponentAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new JobTargetComponent { });
            SetComponentEnabled<JobTargetComponent>(entity, false);
        }
    }
}
