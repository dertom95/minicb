using System;

namespace Data {
    /// <summary>
    /// ResourceTypes. 
    /// Enforce byte for compact usage in components
    /// </summary>
    public enum ResourceType : byte {
        Wood,   // needed to build houses or wood-objects
        Stone,  // needed to build houses
        Food,   // needed to feed settlers
        Tools  // needed to speed up jobs 
    }

    /// <summary>
    /// All resource prefab types
    /// The resource-prefabs itself are grouped in ResourceTypes 
    /// </summary>
    public enum ResourcePrefabType : byte {
        Tree,   // Tree Prefab (res: wood)
        Stone,  // Stone Prefab (res: stone)
        Mushroom // Mushroom Prefab (res: food)
    }

}