using Unity.Entities;

namespace Components {
    public struct GrowingComponent : IComponentData, IEnableableComponent {
        public float growTimePerSecond;
    }
}