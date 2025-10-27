using Data;
using Manager;
using System;
using System.Diagnostics;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using static Manager.InputManager;


namespace Manager {
    /// <summary>
    /// Selection mode. On click => try to select what ever is under the mouse
    /// </summary>
    public class StateSelectionMode : IState<InputManagerContext>, IUpdatableState<InputManagerContext> {
        private EntityManager em;
        private IPhysicsManager physicsManager;

        private Entity previewEntity;
        public Entity PreviewEntity => previewEntity;

        public void OnStateEnter(InputManagerContext ctx, object input) {
        }

        public void OnStateExit() {
        }

        public bool OnUpdate(InputManagerContext ctx) {
            if (ctx.physicsManager.TryToPickRaycast(out Unity.Physics.RaycastHit hit)) {
                UnityEngine.Debug.Log("SelectionMode: Picked: " + hit.Entity);
            }
            return true; // keep running
        }
    }

}

