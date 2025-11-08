using Unity.Entities;

namespace Components {
    /// <summary>
    /// This is a playercomponent that stores the players max and current playeramounts
    /// </summary>
    public struct SettlerAmountComponent : IComponentData {
        /// <summary>
        /// The amount of settlers at the moment
        /// </summary>
        public int currentSettlerAmount;

        /// <summary>
        /// The (theoretical) max amount of players
        /// This amount is relevant for creating new setterls
        /// If for some reason a house will be destroyed this value would be decreased
        /// but the settlers wouldn't be terminated. Only no new ones would be spawned
        /// </summary>
        public int maxSettlerAmount;
    }
}