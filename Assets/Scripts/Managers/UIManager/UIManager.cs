using Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Manager {
    public class UIManager : IManager{
        private static UIManager instance = new UIManager();
        public static UIManager Instance => instance;

        /// <summary>
        /// ScriptableObject to expose uiData
        /// </summary>
        // TODO use another approach!?
        private UIData uiData;

        private UIManager() { }

        public void Init() {
            uiData = Resources.Load<UIData>("UIData");

            DataManager.Instance.RegisterInventoryChangedEvent(OnInventoryChange);
            InputManager.Instance.EventSelectedBuildingChanged += OnSelectedBuildingChanged;
        }

        public void Dispose() {
            DataManager.Instance.UnregisterInventoryChangedEvent(OnInventoryChange);
            InputManager.Instance.EventSelectedBuildingChanged -= OnSelectedBuildingChanged;
        }

        private void OnInventoryChange(object sender, Dictionary<Data.ResourceType, int> inv) {
            // This is a very basic first approach
            uiData.wood = inv[Data.ResourceType.Wood];
            uiData.stone = inv[Data.ResourceType.Stone];
            uiData.food = inv[Data.ResourceType.Food];
            uiData.tools = inv[Data.ResourceType.Tools];
        }

        private void OnSelectedBuildingChanged(object sender, BuildingType buildingType) {
            uiData.selectedBuilding = buildingType.ToString();
        }

        /// <summary>
        /// Check if the cursor is over UI-Elements
        /// </summary>
        /// <returns></returns>
        public bool IsMouseOverUI() {
            var root = BuildingButtonSpawner.Instance.uiDocument.rootVisualElement;
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            float flippedY = Screen.height - mousePosition.y;
            Vector2 mousePos = new Vector2(mousePosition.x, Screen.height - mousePosition.y);

            // Pick the topmost element under the mouse
            VisualElement elementUnderMouse = root.panel.Pick(mousePos);

            if (elementUnderMouse != null) {
                //Debug.Log($"{mousePosition} : Mouse is over UI element: {elementUnderMouse.name}");
                return true;
            } else {
                //Debug.Log($"{mousePosition} : Mouse is NOT over any UI element");
                return false;
            }
        }
    }
}