using Components.Tags;
using Data;
using Manager;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingButtonSpawner : MonoBehaviour {
    public static BuildingButtonSpawner Instance;

    public VisualTreeAsset buttonTemplate; 
    public UIDocument uiDocument;
    public VisualElement uiEntityInspector;
    public Toggle uiEntityEnabledToggle;

    private Entity inspectorEntity;
    public Entity InspectorEntity => inspectorEntity;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        var root = uiDocument.rootVisualElement;
        var container = root.Q<VisualElement>("button-container");
        uiEntityInspector = root.Q<VisualElement>("building-inspector");

        uiEntityEnabledToggle = root.Q<Toggle>("entity-enabled");

        if (uiEntityEnabledToggle != null) {
            // Register callback for value changes
            uiEntityEnabledToggle.RegisterValueChangedCallback(evt => {
                Assert.AreNotEqual(Entity.Null, inspectorEntity);

                bool isEnabled = evt.newValue;
                Debug.Log($"Entity Enabled toggled: {isEnabled} Entity:{Mgr.inputManager.CurrentSelectedEntity}");

                EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
                em.SetComponentEnabled<TagWorking>(inspectorEntity, isEnabled);
            });
        } else {
            Debug.LogWarning("Toggle 'entity-enabled' not found in UI.");
        }

        // TODO: this must be somehow be configurable. At least in the configs
        BuildingType[] buttonLabels = { BuildingType.gatherer, BuildingType.mason, BuildingType.woodcutter, BuildingType.woodworker, BuildingType.tree_nursery, BuildingType.house };

        for (int i = 0; i < buttonLabels.Length; i++) {
            int index = i; // Capture for closure
            // Instantiate the template
            var buttonInstance = buttonTemplate.Instantiate();
            var button = buttonInstance.Q<Button>("button");
            var label = buttonInstance.Q<Label>("button-label");

            BuildingType currentBuildingType = buttonLabels[i];
            label.text = buttonLabels[i].ToString();

            // Assign an action to the button
            button.clicked += () => {
                Mgr.inputManager.StartBuildMode(currentBuildingType);
                Debug.Log($"Button '{buttonLabels[index]}' clicked! Index: {index}");
                // Place your custom action here
            };

            container.Add(buttonInstance);
        }

        SetInspectorVisible(false);
    }

    public void SetInspectorEntity(Entity entity) {
        inspectorEntity = entity;

        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (entity!=Entity.Null && em.HasComponent<TagWorking>(entity)) {
            uiEntityEnabledToggle.style.display = DisplayStyle.Flex;
            bool working = em.IsComponentEnabled<TagWorking>(entity);
            uiEntityEnabledToggle.SetEnabled(working);
        } else {
            uiEntityEnabledToggle.style.display = DisplayStyle.None;
        }
    }


    public void SetInspectorVisible(bool visible) {
        uiEntityInspector.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
