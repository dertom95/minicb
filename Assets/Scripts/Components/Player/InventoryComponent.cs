using UnityEngine;
using Unity.Collections;
using Unity.Entities;


namespace Components {
    /// <summary>
    /// Stores resource amount (hardcoded), limited to [0..65535]
    /// </summary>
    public struct InventoryComponent : IComponentData {
        public ushort wood;
        public ushort stone;
        public ushort food;
    }
}
