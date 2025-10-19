using Manager;
using NUnit.Framework;
using System.Collections.Generic;

/// <summary>
/// Lightweight game framework. Handling managers
/// </summary>
public class MiniCBMain {

    private List<IManager> managers;
    
    public void Init() {
        InitManagers();
    }

    private void InitManagers() {
        managers = new List<IManager>();
        managers.Add(new InputManager());
    }

    public void Update(float dt) {
        UpdateManagers(dt);
    }

    private void UpdateManagers(float dt) {
        foreach (IManager manager in managers) {
            manager.Update(dt);
        }
    }
}