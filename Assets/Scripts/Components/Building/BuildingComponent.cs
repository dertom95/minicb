using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Data;
using System;


namespace Components {
    /// <summary>
    /// ResourceComponent
    /// </summary>
    [Serializable]
    public struct BuildingComponent : IComponentData {
        /// <summary>
        /// The buildingtype (stored as byte, see enum-signature)
        /// </summary>
        public Data.BuildingType buildingType; // byte

        /// <summary>
        /// The state this building is working at the moment
        /// </summary>
        public Data.BuildingState currentState; // byte

        /// <summary>
        /// The amount of collect iterations left until that resource will be destroyed
        /// </summary>
        public BuildingCosts buildingCosts; // ushort

        //TODO: pull all job-related data in a component of its own
        /// <summary>
        /// The job type
        /// </summary>
        public JobType jobType;

        /// <summary>
        /// The max amount of current jobs (running and pending)
        /// </summary>
        public byte maxJobs;

        /// <summary>
        /// The current amount of running and pending jobs spawned by this building
        /// </summary>
        public byte currentJobAmount;

        /// <summary>
        /// The amount of seconds to work for a job to finish
        /// </summary>
        public byte jobDurationInSeconds;
    }
}
