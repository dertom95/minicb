using Data;
using Unity.Entities;

namespace Components {
    public struct ResourceToResourceComponent : IComponentData {
    }

    public struct RTRInputResource : IBufferElementData {
        public ResourceAmount resIn;    
    }

    public struct RTROutputResource: IBufferElementData {
        public ResourceAmount resOut;
    }
}