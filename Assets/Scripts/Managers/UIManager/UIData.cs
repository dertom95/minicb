using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "MiniCB/uidata")]
public class UIData : ScriptableObject {
    [Header("Runtime-Data")]
    public int wood = 0;
    public int stone = 0;
    public int food = 0;
    public int tools = 0;

    public int currentSettlerAmount = 0;
    public int maxSettlerAmount = 0;

    public string selectedBuilding;

    public string currentBuildingName = "";
}
