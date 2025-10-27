namespace Manager {
    using Components;
    using Data;
    using System;
    using System.Collections.Generic;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputManager : IManagerUpdateable, IInputManager {

        public event EventHandler<BuildingType> EventSelectedBuildingChanged;

        public enum InputMode {
            Selection, BuildMode
        }

        public struct InputManagerContext {
            public InputMode mode;
            public IPhysicsManager physicsManager;
            public InputManager inputManager;
            public World world;
        }

        public StateMachine<InputMode, InputManagerContext> stateMachine;

        private InputManagerContext ctx;

        private EntityManager entityManager;


        public InputManager() {
        }

        public void Init() {
            World world = World.DefaultGameObjectInjectionWorld;
            ctx = new InputManagerContext {
                world = world,
                physicsManager = Mgr.physicsManager
            };

            entityManager = world.EntityManager;
            stateMachine = new StateMachine<InputMode,InputManagerContext>(ctx, InputMode.Selection);
            stateMachine.RegisterState(InputMode.Selection, new StateSelectionMode());
            stateMachine.RegisterState(InputMode.BuildMode, new StateBuildMode());
            stateMachine.ChangeState(InputMode.Selection);
        }

        public void Dispose() {
        }

        public void Update(float dt) {
            stateMachine.Update();
        }

        /// <summary>
        /// Check if the mouseButton is down button is pressed
        /// Optionally ignore the button if the mouse was abouve ui-element
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="ignoreButtonIfOverUI"></param>
        /// <returns></returns>
        public bool IsMouseButtonDown(int btn, bool ignoreButtonIfOverUI = true) {
            return Input.GetMouseButtonDown(btn) && (!ignoreButtonIfOverUI || !Mgr.uiManager.IsMouseOverUI());
        }

        public void StartBuildMode(BuildingType buildingType) {
            stateMachine.ChangeState(InputMode.BuildMode, buildingType);

            EventSelectedBuildingChanged?.Invoke(this, buildingType);
        }
    }

}

