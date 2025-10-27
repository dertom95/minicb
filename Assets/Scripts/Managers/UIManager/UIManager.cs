using Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Manager {
    public class UIManager : IManager, IUIManager {
        /// <summary>
        /// ScriptableObject to expose uiData
        /// </summary>
        // TODO use another approach!?
        private UIData uiData;

        public UIManager() { }

        public void Init() {
            uiData = Resources.Load<UIData>("UIData");

            Mgr.dataManager.RegisterInventoryChangedEvent(OnInventoryChange);
            Mgr.inputManager.EventSelectedBuildingChanged += OnSelectedBuildingChanged;
        }

        public void Dispose() {
            Mgr.dataManager.UnregisterInventoryChangedEvent(OnInventoryChange);
            Mgr.inputManager.EventSelectedBuildingChanged -= OnSelectedBuildingChanged;
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
            var uiDoc = BuildingButtonSpawner.Instance?.uiDocument;
            if (uiDoc == null) {
                Debug.LogWarning("UIDocument is null");
                return false;
            }

            var root = uiDoc.rootVisualElement;
            if (root == null) {
                Debug.LogWarning("rootVisualElement is null");
                return false;
            }

            if (root.panel == null) {
                Debug.LogWarning("root.panel is null");
                return false;
            }

            //var root = BuildingButtonSpawner.Instance.uiDocument.rootVisualElement;
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            float flippedY = Screen.height - mousePosition.y;
            //            float flippedY = mousePosition.y;
            Vector2 mousePos = new Vector2(mousePosition.x, flippedY);
            Vector2 ppos = RuntimePanelUtils.ScreenToPanel(root.panel, mousePos);
            // Pick the topmost element under the mouse
            VisualElement elementUnderMouse = root.panel.Pick(ppos);

            if (elementUnderMouse != null) {
                Debug.Log($"{mousePos}|{ppos} : Mouse is over UI element: {elementUnderMouse.name}");
                return true;
            } else {
                Debug.Log($"{mousePos}|{ppos} : Mouse is NOT over any UI element");
                return false;
            }
        }
    }
}