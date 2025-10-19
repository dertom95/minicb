using Data;
using System;
using Unity.Entities;
using UnityEngine;

namespace Components {
    [Serializable]
    public struct EntityToResourceComponent : IComponentData {
        /// <summary>
        /// The resource type this entity is looking for in its FieldOfInfluence
        /// </summary>
        public ResourceType searchResourceType; // byte
    }
}