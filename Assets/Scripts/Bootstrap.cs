using Manager;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour {
    MiniCBMain main;
    public void Awake() {
        main = new MiniCBMain();

        Mgr.Init();

        main.Init(new List<IManager>() {
            Config.Instance,
            Mgr.assetManager,
            Mgr.uiManager,
            Mgr.inputManager,
            Mgr.dataManager,
            Mgr.buildingManager,
            Mgr.jobManager,
            Mgr.settlerManager,
            Mgr.animationManager
        });
    }

    public void Update() {
        float dt = Time.deltaTime;
        main.Update(dt);
    }
}