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

        // Example data for buttons
        BuildingType[] buttonLabels = { BuildingType.gatherer, BuildingType.mason, BuildingType.woodcutter, BuildingType.woodworker, BuildingType.tree_nursery };

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
                Mgr.inputManager.SetCurrentBuilding(currentBuildingType);
                Debug.Log($"Button '{buttonLabels[index]}' clicked! Index: {index}");
                // Place your custom action here
            };

            container.Add(buttonInstance);
        }
    }
}
