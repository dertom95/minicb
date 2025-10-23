using Components;
using Components.Tags;
using Data;
using Unity.Entities;
using UnityEngine;

public class SettlerAuthoring : MonoBehaviour
{
    /// <summary>
    /// The walk speed
    /// </summary>
    public float walkSpeed;

    /// <summary>
    /// The work speed. 
    /// </summary>
    public float workSpeed;

    /// <summary>
    /// SettlerType
    /// </summary>
    public SettlerType settlerType;

    /// <summary>
    /// A bitmask of jobs this settler can accept
    /// </summary>
    public JobType acceptJobs;

    [Header("Bouncy Animation")]
    public float bounceheight;
    public float walkBounceSpeed;
    public float workBounceSpeed;

    public class Baker : Baker<SettlerAuthoring> {
        public override void Bake(SettlerAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SettlerComponent {
                currentJob = Entity.Null,
                acceptJobs = authoring.acceptJobs,
                settlerType = authoring.settlerType,
                walkSpeed = authoring.walkSpeed,
                workSpeed = authoring.workSpeed
            });
            // add disable worktimer
            AddComponent<WorkTimerComponent>(entity);
            SetComponentEnabled<WorkTimerComponent>(entity, false);

            // add disabled TagWorking
            AddComponent<TagWorking>(entity);
            SetComponentEnabled<TagWorking>(entity, false);

            AddComponent(entity, new AnimationStateComponent { 
                animationState = Data.AnimationState.idle
            });

            AddComponent(entity, new JumpAnimationComponent { 
                height = authoring.bounceheight,
                walkSpeed = authoring.walkBounceSpeed,
                workSpeed = authoring.workBounceSpeed
            });

            AddBuffer<ResourceBufferElement>(entity);
        }
    }
}
