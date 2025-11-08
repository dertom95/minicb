using Data;
using Manager;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingButtonSpawner : MonoBehaviour {
    public static BuildingButtonSpawner Instance;

    public VisualTreeAsset buttonTemplate; 
    public UIDocument uiDocument;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        var root = uiDocument.rootVisualElement;
        var container = root.Q<VisualElement>("button-container");

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
    }
}
