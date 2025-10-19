namespace Data {
    /// <summary>
    /// ResourceTypes. 
    /// Enforce byte for compact usage in components
    /// </summary>
    public enum ResourceType : byte { 
        Wood,   // needed to build houses or wood-objects
        Stone,  // needed to build houses
        Food    // needed to feed settlers
    }

    /// <summary>
    /// BuildingTypes. 
    /// Enforce byte for compact usage in components
    /// </summary>
    public enum BuildingType : byte { 
        gatherer,       // collects berries and mushrooms
        house,          // regualtes the max amount of settlers
        mason,          // collects stone
        tree_nursery,   // put tree plants
        woodcutter,     // collects wood from trees
        woodworker      // creates objects from wood
    }

    /// <summary>
    /// SettlerStates 
    /// Enforce byte for compact usage in components
    /// </summary>
    public enum SettlerState : byte { 
        Idle,
        Moving,
        Working
    }

    /// <summary>
    /// BuildingStates. 
    /// Enforce byte for compact usage in components
    /// </summary>
    public enum BuildingState : byte {
        UnderConstruction,
        Built,
        Paused,
        LookingForRequirements,
        Working
    }
}