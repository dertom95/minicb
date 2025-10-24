using Unity.Entities;

namespace Components {
    /// <summary>
    /// Component to store a generic amount of iterationsLeft
    /// Optionally destroys entity it is attached to when iterationsLeft==0
    /// </summary>
    public struct IterationsComponent : IComponentData, IEnableableComponent {
        public byte iterationsLeft;
    }

    /// <summary>
    /// Tag to indicate that this entity should be automatically been destroyed if interationsLeft==0
    /// </summary>
    public struct TagIterationAutoDestroy : IComponentData, IEnableableComponent { }
}