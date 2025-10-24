using UnityEngine;
using Unity.Collections;
using Unity.Entities;


namespace Components {
    /// <summary>
    /// ResourceComponent
    /// </summary>
    public struct ResourceComponent : IComponentData {
        /// <summary>
        /// The resourceType (stored as byte, see enum-signature)
        /// </summary>
        public Data.ResourceType resourceType;

        /// <summary>
        /// The amount of resources added to the inventory per iteration
        /// </summary>
        public byte resourceAmountPerIteration;
    }
}
