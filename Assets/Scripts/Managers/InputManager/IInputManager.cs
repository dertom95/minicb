using Data;
using System;

namespace Manager {
    public interface IInputManager : IManager {
        event EventHandler<BuildingType> EventSelectedBuildingChanged;

        void SetCurrentBuilding(BuildingType buildingType);
    }
}