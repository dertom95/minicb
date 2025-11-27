using Data;
using Manager;
using System;
using System.Diagnostics;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using static Manager.InputManager;


namespace Manager {
    /// <summary>
    /// Selection mode. On click => try to select what ever is under the mouse
    /// </summary>
    public class StateSelectionMode : IState<InputManagerContext>, IUpdatableState<InputManagerContext> {
        private EntityManager em;
        private IPhysicsManager physicsManager;

        CollisionFilter filter = new CollisionFilter {
            BelongsTo = ~0u,               // Belongs to all layers
            CollidesWith = 1<<Config.LAYER_BUILDINGS | 1<<Config.LAYER_RESOURCE,       
            GroupIndex = 0
        };

        public void OnStateEnter(InputManagerContext ctx, object input) {
        }

        public void OnStateExit() {
        }

        public bool OnUpdate(ref InputManagerContext ctx) {
            bool clicked = Mgr.inputManager.IsMouseButtonDown(0);
            if (clicked) {
                if (ctx.physicsManager.TryToPickRaycast(out Unity.Physics.RaycastHit hit, filter)) {
                    ctx.selectedEntity = hit.Entity;
                    ctx.inputManager.TriggerSelectedEntityChanged();
                } else {
                    ctx.selectedEntity = Entity.Null;
                    ctx.inputManager.TriggerSelectedEntityChanged();
                }
            }
            return true; // keep running
        }
    }

}

