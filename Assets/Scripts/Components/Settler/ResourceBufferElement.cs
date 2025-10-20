using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Data;


namespace Components {
    [InternalBufferCapacity(2)] // Or whatever default you want
    public struct ResourceBufferElement : IBufferElementData {
        public ResourceType type;
        public ushort amount;
    }
}
