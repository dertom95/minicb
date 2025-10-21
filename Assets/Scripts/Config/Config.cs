using Data;
using Manager;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Config : IManager {
    private static Config instance = new Config();
    public static Config Instance => instance;

    private Config() { }

    private ConfigSO configSO;

    /// <summary>
    /// The initial inventory resources
    /// </summary>
    public List<ResourceAmount> InitialInventory => configSO.initialInventory;

    /// <summary>
    /// The unity layer where all resource entities are stored
    /// </summary>
    public const int LAYER_RESOURCE = 3;

    public void Init() {
        configSO = Resources.Load<ConfigSO>("Config");
        Assert.IsNotNull(configSO);
    }

    public void Dispose() {
    }

}