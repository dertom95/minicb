using System;

namespace Data {
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

}