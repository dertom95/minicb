using System;
using Unity.Entities;
using UnityEngine;

namespace Data {
    [Serializable]
    public struct BuildingCosts : IComponentData {
        /// <summary>
        /// The amount of wood needed to build this building
        /// </summary>
        public byte wood;
        /// <summary>
        /// The amount of stone needed to build this building
        /// </summary>
        public byte stone;
        /// <summary>
        /// The amount of seconds needed to finish this building
        /// </summary>
        [Tooltip("The amount of seconds needed to finish this building")]
        public byte timeInSeconds;
    }
}