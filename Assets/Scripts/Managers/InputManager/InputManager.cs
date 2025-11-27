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
    using UnityEngine.Assertions;
    using UnityEngine.InputSystem;

    public class InputManager : IManagerUpdateable, IInputManager {

        public event EventHandler<BuildingType> EventSelectedBuildBuildingChanged;
        public event EventHandler<Entity> EventSelectedEntityChanged;

        public enum InputMode {
            Selection, BuildMode
        }

        public struct InputManagerContext {
            public InputMode mode;
            public Entity selectedEntity;
            public IPhysicsManager physicsManager;
            public InputManager inputManager;
            public World world;
        }

        public StateMachine<InputMode, InputManagerContext> stateMachine;

        private EntityManager entityManager;
        private ref InputManagerContext CTX => ref stateMachine.GetContext();

        public Entity CurrentSelectedEntity => CTX.selectedEntity;

        public InputManager() {
        }

        public void Init() {
            World world = World.DefaultGameObjectInjectionWorld;
            InputManagerContext initialContext = new InputManagerContext {
                world = world,
                physicsManager = Mgr.physicsManager,
                inputManager = this
            };

            entityManager = world.EntityManager;
            stateMachine = new StateMachine<InputMode,InputManagerContext>(initialContext, InputMode.Selection);
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

            EventSelectedBuildBuildingChanged?.Invoke(this, buildingType);
        }

        public void TriggerSelectedEntityChanged() {
            Assert.AreEqual(InputMode.Selection, CTX.mode);

            EventSelectedEntityChanged?.Invoke(this, CTX.selectedEntity);
        }
    }

}

