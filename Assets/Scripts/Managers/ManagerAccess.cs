using Manager;
using UnityEngine.Assertions;

/// <summary>
/// The poor guy's dependency injection :D
/// Not sure about that approach :D Also thought about TypeMap-Approach to let the MainCB dynamically set the Managers. Or using some kind of
/// Dependency-Injection-Lib, but in the end I want only to be able to use Mock-Managers(if needed)
/// 
/// </summary>
public static class Mgr {
    public enum ManagerSetup : byte {
        None,
        Runtime,
        UnitTesting
    }

    public static IAnimationManager animationManager;
    public static IAssetManager assetManager;
    public static IBuildingManager buildingManager;
    public static IDataManager dataManager;
    public static IInputManager inputManager;
    public static IJobManager jobManager;
    public static ISettlerManager settlerManager;
    public static IUIManager uiManager;
    public static IPhysicsManager physicsManager;

    private static ManagerSetup currentSetup = ManagerSetup.None;

    public static void CleanupManagers() {
        physicsManager?.Dispose();
        animationManager?.Dispose();
        assetManager?.Dispose();
        buildingManager?.Dispose();
        dataManager?.Dispose();
        inputManager?.Dispose();
        jobManager?.Dispose();
        settlerManager?.Dispose();
        uiManager?.Dispose();

        animationManager = null;
        assetManager = null;
        buildingManager = null;
        dataManager = null;
        inputManager = null;
        jobManager = null;
        settlerManager = null;  
        uiManager = null;
    }

    public static void Init(ManagerSetup mgrSetup = ManagerSetup.Runtime) {
        CleanupManagers();
        switch (mgrSetup) {
            case ManagerSetup.Runtime: 
                InitRuntime(); 
                break;
            case ManagerSetup.UnitTesting: 
                InitUnitTesting(); 
                break;
            default:
                throw new System.Exception("Unknown ManagerSetup");
        }
    }

    private static void InitRuntime() {
        physicsManager = new PhysicsManager();
        animationManager = new AnimationManager();
        assetManager = new AssetManager();
        buildingManager = new BuildingManager();
        dataManager = new DataManager();
        inputManager = new InputManager();
        jobManager = new JobManager();
        settlerManager = new SettlerManager();
        uiManager = new UIManager();
    }

    private static void InitUnitTesting() {
        physicsManager = new PhysicsManager();
        animationManager = new AnimationManager();
        assetManager = new AssetManager();
        buildingManager = new BuildingManager();
        dataManager = new DataManager();
        jobManager = new JobManager();
        settlerManager = new SettlerManager();
    }


}