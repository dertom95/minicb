using Components;
using Data;
using System;
using System.Collections.Generic;
using Unity.Entities;
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
        private EntityManager entityManager;
        private Entity currentSelectedEntity = Entity.Null;

        public UIManager() { }

        public void Init() {
            uiData = Resources.Load<UIData>("UIData");
            uiData.currentSettlerAmount = 0;
            uiData.maxSettlerAmount = 0;
            uiData.currentBuildingName = "unknown";

            Mgr.dataManager.SubscribeToInventoryChangedEvent(OnInventoryChange);
            Mgr.inputManager.EventSelectedBuildBuildingChanged += OnSelectedBuildBuildingTypeChanged;
            Mgr.inputManager.EventSelectedEntityChanged += OnSelectedEntityChanged;
            Mgr.settlerManager.EventSettlerAmountChanged += OnSettlerAmountChanged;

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public void Dispose() {
            Mgr.dataManager.UnsubscribeFromInventoryChangedEvent(OnInventoryChange);
            Mgr.inputManager.EventSelectedBuildBuildingChanged -= OnSelectedBuildBuildingTypeChanged;
            Mgr.settlerManager.EventSettlerAmountChanged -= OnSettlerAmountChanged;
        }

        private void OnInventoryChange(object sender, Dictionary<Data.ResourceType, int> inv) {
            // This is a very basic first approach
            uiData.wood = inv[Data.ResourceType.Wood];
            uiData.stone = inv[Data.ResourceType.Stone];
            uiData.food = inv[Data.ResourceType.Food];
            uiData.tools = inv[Data.ResourceType.Tools];
        }

        private void OnSelectedBuildBuildingTypeChanged(object sender, BuildingType buildingType) {
            uiData.selectedBuilding = buildingType.ToString();
        }

        public void OnSettlerAmountChanged(object sender, EventArgs e) {
            uiData.maxSettlerAmount = Mgr.settlerManager.GetMaxSettlerAmount();
            uiData.currentSettlerAmount = Mgr.settlerManager.GetCurrentSettlerAmount();
        }

        public void OnSelectedEntityChanged(object sender, Entity entity) {
            BuildingButtonSpawner.Instance.SetInspectorEntity(entity);

            if (entity == Entity.Null) {
                uiData.currentBuildingName = "No Selection";
                BuildingButtonSpawner.Instance.SetInspectorVisible(false);
                return;
            }

            BuildingButtonSpawner.Instance.SetInspectorVisible(true);
            if (entityManager.HasComponent<BuildingComponent>(entity)) {
                BuildingComponent buildingComponent = entityManager.GetComponentData<BuildingComponent>(entity);
                uiData.currentBuildingName = buildingComponent.buildingType.ToString();
            } 
            else if (entityManager.HasComponent<ResourceComponent>(entity)) {
                ResourceComponent resourceComponent = entityManager.GetComponentData<ResourceComponent>(entity);
                uiData.currentBuildingName = resourceComponent.resourceType.ToString();
            } else {
                uiData.currentBuildingName = "Unknown Type";
            }
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