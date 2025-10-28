using Data;
using Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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


    /// <summary>
    /// The unity layer where all resource entities are stored
    /// </summary>
    public const int LAYER_BUILDINGS = 6;

    /// <summary>
    /// The unity layer where all resource entities are stored
    /// </summary>
    public const int LAYER_TERRAIN = 7;

    /// <summary>
    /// The material id to use for to color the visual on error (e.g. collision)
    /// TOOD: Not sure how to reliably get this id in code (I just tested to find it) ;)
    /// </summary>
    public const int MAT_PALETTE_ERROR = -1;
    //public const int MAT_PALETTE_ERROR = 6;

    public void Init() {
        configSO = Resources.Load<ConfigSO>("Config");
        Assert.IsNotNull(configSO);
    }

    public void Dispose() {
    }

}