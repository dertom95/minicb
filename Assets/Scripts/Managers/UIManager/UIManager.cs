using System.Collections.Generic;
using UnityEngine;

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

            DataManager.Instance.EventInventoryChanged += OnInventoryChange;
        }

        public void Dispose() {
            DataManager.Instance.EventInventoryChanged -= OnInventoryChange;
        }

        private void OnInventoryChange(object sender, Dictionary<Data.ResourceType, int> inv) {
            // This is a very basic first approach
            uiData.wood = inv[Data.ResourceType.Wood];
            uiData.stone = inv[Data.ResourceType.Stone];
            uiData.food = inv[Data.ResourceType.Food];
            uiData.tools = inv[Data.ResourceType.Tools];
        }
    }
}