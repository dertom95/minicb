using Data;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "minicb/config")]
public class ConfigSO : ScriptableObject {
    public List<ResourceAmount> initialInventory = new List<ResourceAmount> ();
    public int initialAmountSettlers = 3;
}