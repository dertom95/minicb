namespace Data {
    /// <summary>
    /// resource amount (amount restricted to [0..65535] for now)
    /// </summary>
    public struct ResourceAmount {
        ResourceType resourceType;
        ushort resourceAmount;
    }
}