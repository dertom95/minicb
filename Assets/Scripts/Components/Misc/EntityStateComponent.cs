using Components.Decorators;
using Unity.Entities;

namespace Components {
    /// <summary>
    /// Component to control state changes. 
    /// 
    /// Systems or Managers can query this Component
    /// </summary>
    public struct EntityStateComponent : IComponentData, IEnableableComponent, IRemoveForPreviewEntity {
        public enum EntityStateType : byte {
            OnCreation,    // Entity is being created 
            Created,       // Entity has been created
            OnPause,       // Entity is going to be paused
            Paused,        // Entity is paused
            OnDestruction, // Entity is about to be destroyed
            Destroyed      // Entity is destroyed
        }
        
        public EntityStateType currentState;
    }
}