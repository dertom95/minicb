using System;

namespace Data {

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
    /// BuildingStates. 
    /// Enforce byte for compact usage in components
    /// </summary>
    /// TODO: Rethink this. The state should be handled via Tags
    public enum BuildingState : byte {
        Preview,
        UnderConstruction,
        Built
    }
}