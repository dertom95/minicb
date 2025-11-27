using Data;
using System;
using Unity.Entities;

namespace Manager {
    public interface IInputManager : IManager {
        public Entity CurrentSelectedEntity { get; }

        /// <summary>
        /// Event that in building-mode the selected buildingtype for build has changed
        /// </summary>
        event EventHandler<BuildingType> EventSelectedBuildBuildingChanged;

        /// <summary>
        /// In selection-mode the selected entity changed (can be Entity.Null)
        /// </summary>
        event EventHandler<Entity> EventSelectedEntityChanged;

        /// <summary>
        /// Check if the mouseButton is down button is pressed
        /// Optionally ignore the button if the mouse was abouve ui-element
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="ignoreButtonIfOverUI"></param>
        /// <returns></returns>
        bool IsMouseButtonDown(int btn, bool ignoreButtonIfOverUI = true);

        /// <summary>
        /// Start build-mode for the specified buildingtype
        /// </summary>
        /// <param name="buildingType"></param>
        void StartBuildMode(BuildingType buildingType);
    }
}