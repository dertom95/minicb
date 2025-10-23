using System;

namespace Data {
    /// <summary>
    /// JobTypes that can be executed by settlers
    /// </summary>
    [Flags]
    public enum JobType : byte {
        Construction = 1 << 0,
        EntityToResource = 1 << 1,
        ConvertResource = 1 << 2,
        SpawnEntity = 1 << 3,
        Attack = 1 << 4
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

}