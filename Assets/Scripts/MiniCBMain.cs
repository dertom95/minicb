using Manager;
using NUnit.Framework;
using System.Collections.Generic;

/// <summary>
/// Lightweight game framework. Handling managers
/// </summary>
public class MiniCBMain {

    public static bool initialized = false;

    private List<IManager> allManagers = new List<IManager>();
    private List<IManagerUpdateable> updateableManagers = new List<IManagerUpdateable>();

    /// <summary>
    /// Init minicb and its managers. Use baseOnly in test-scenario to ignore 'unwanted' managers
    /// </summary>
    /// <param name="baseOnly"></param>
    public void Init(List<IManager> managers) {
        foreach (IManager manager in managers) {
            RegisterManager(manager);
        }

        // call the init-methods
        InitManagers();

        initialized = true;
    }

    private void RegisterManager(IManager manager) {
        Assert.IsNotNull(manager);

        allManagers.Add(manager);
        if (manager is IManagerUpdateable updateableManager) {
            updateableManagers.Add(updateableManager);
        }
    }

    public void Update(float dt) {
        UpdateManagers(dt);
    }

    /// <summary>
    /// Update all registered Managers with the specified deltaTime
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateManagers(float dt) {
        foreach (IManagerUpdateable manager in updateableManagers) {
            manager.Update(dt);
        }
    }

    /// <summary>
    /// Call Init-Method on all registered managers
    /// </summary>
    private void InitManagers() {
        foreach (IManager manager in allManagers) {
            manager.Init();
        }
    }

    /// <summary>
    /// Disposes all registered Managers
    /// </summary>
    public void DisposeManagers() {
        Mgr.CleanupManagers();
    }
}