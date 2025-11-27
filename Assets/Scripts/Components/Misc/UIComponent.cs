using Unity.Entities;

namespace Components {
    public struct UIComponent : IComponentData {
        public Entity selectedEntity;
    }
}
