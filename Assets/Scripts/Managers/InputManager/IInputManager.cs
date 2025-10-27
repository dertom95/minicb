using Data;
using System;

namespace Manager {
    public interface IInputManager : IManager {
        event EventHandler<BuildingType> EventSelectedBuildingChanged;

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