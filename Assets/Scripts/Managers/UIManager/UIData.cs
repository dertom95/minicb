using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "minicb/uidata")]
public class UIData : ScriptableObject {
    [Header("Runtime-Data")]
    public int wood = 0;
    public int stone = 0;
    public int food = 0;
    public int tools = 0;

    public string selectedBuilding;
}
