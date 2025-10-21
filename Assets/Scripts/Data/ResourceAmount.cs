using System;

namespace Data {
    /// <summary>
    /// resource amount (amount restricted to [0..65535] for now)
    /// </summary>
    [Serializable]
    public struct ResourceAmount {
        public ResourceType resourceType;
        public ushort resourceAmount;
    }
}