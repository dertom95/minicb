using Components;
using Data;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class WorkTimerComponentAuthoring : MonoBehaviour
{
    /// <summary>
    /// The amount of time needed to finish on work-cycle
    /// </summary>
    public float workTimeIntervalInSecs;

    public class Baker : Baker<WorkTimerComponentAuthoring> {
        public override void Bake(WorkTimerComponentAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new WorkTimerComponent {
                workTimeTotal = authoring.workTimeIntervalInSecs,
                currentTime = 0
            });
        }
    }
}
