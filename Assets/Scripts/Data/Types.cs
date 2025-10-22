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
        Working
    }

    /// <summary>
    /// SettlerTypes
    /// </summary>
    public enum SettlerType : byte {
        Child,
        Worker,
        Warrior
    }

    /// <summary>
    /// JobTypes that can be executed by settlers
    /// </summary>
    [Flags]
    public enum JobType : byte {
        Construction        = 1 << 0,
        EntityToResource = 1 << 1,
        ConvertResource     = 1 << 2,
        SpawnEntity         = 1 << 3,
        Attack              = 1 << 4
    }

    /// <summary>
    /// JobState
    /// </summary>
    public enum JobState : byte {
        /// <summary>
        /// not assigned to settler
        /// </summary>
        free,
        /// <summary>
        /// TransitionState at the very beginning of the job after assignment
        /// </summary>
        Start,
        /// <summary>
        /// settler is moving to target
        /// </summary>
        MovingToTarget,
        /// <summary>
        /// TransitionState after reaching the Target
        /// </summary>
        ReachedTarget,
        /// <summary>
        /// settler is doing the work
        /// </summary>
        Working,
        /// <summary>
        /// TransitionState after finished Working
        /// </summary>
        FinishedWorking,
        /// <summary>
        /// settler is moving back to owner (if needed)
        /// </summary>
        MovingToOwner,
        /// <summary>
        /// TransitionState after MovingToOwner finished
        /// </summary>
        ReachedMovingToOwner,
        /// <summary>
        /// Something went wrong and the job is going to be terminated
        /// </summary>
        Quit

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