using Unity.Entities;

namespace Components {

    /// <summary>
    /// Static field of influes. No always relative to entity-position
    /// </summary>
    public struct FieldOfInfluenceComponent : IComponentData {
        /// <summary>
        /// The radius around the entity
        /// </summary>
        public float radius;
    }
}