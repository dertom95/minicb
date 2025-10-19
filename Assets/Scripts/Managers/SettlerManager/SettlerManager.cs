using Components;
using Components.Tags;
using Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Manager {
    /// <summary>
    /// Settler-Specific functions
    /// </summary>
    public class SettlerManager : IManager {
        private static readonly SettlerManager instance = new SettlerManager();

        public static SettlerManager Instance => instance;

        private EntityManager entityManager;
        private DataManager dataManager;

        private SettlerManager() {
        }

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            dataManager = DataManager.Instance;
        }

        public void Dispose() {
        }

    }
}