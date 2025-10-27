using Data;
using Manager;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Scenes;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Unity.Entities.Tests {

    /// <summary>
    /// Used ECSTestFixture as base
    /// </summary>
    public abstract class TestBase {
        protected MiniCBMain _minicbMain;

        protected World m_PreviousWorld;
        protected World World;
        protected PlayerLoopSystem m_PreviousPlayerLoop;
        protected EntityManager entityManager;
        protected EntityManager.EntityManagerDebug m_ManagerDebug;

        protected int StressTestEntityCount = 1000;
        private bool JobsDebuggerWasEnabled;
        protected SceneSystem sceneSystem;
        protected Dictionary<GameObject, Entity> goEntityLookup;

        [SetUp]
        public virtual void Setup() {
            // unit tests preserve the current player loop to restore later, and start from a blank slate.
            m_PreviousPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());

            m_PreviousWorld = World.DefaultGameObjectInjectionWorld;
            World = World.DefaultGameObjectInjectionWorld = new World("Test World");
            World.UpdateAllocatorEnableBlockFree = true;
            entityManager = World.EntityManager;
            m_ManagerDebug = new EntityManager.EntityManagerDebug(entityManager);
            var sceneSystem = World.GetOrCreateSystem<SceneSystem>();

            var simulationGroup = World.GetOrCreateSystemManaged<SimulationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(sceneSystem);

            // Many ECS tests will only pass if the Jobs Debugger enabled;
            // force it enabled for all tests, and restore the original value at teardown.
            JobsDebuggerWasEnabled = JobsUtility.JobDebuggerEnabled;
            JobsUtility.JobDebuggerEnabled = true;

            //JobsUtility.ClearSystemIds();
            
            // Initialize new Manager Instances for Testing
            Mgr.Init(Mgr.ManagerSetup.UnitTesting);

            _minicbMain = new MiniCBMain();
            _minicbMain.Init(GetInitializationManagers());

            goEntityLookup = new Dictionary<GameObject, Entity>();

#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !DISABLE_ENTITIES_JOURNALING
            // In case entities journaling is initialized, clear it
            EntitiesJournaling.Clear();
#endif
        }

        protected void InitDefault() {
            //LoadPrefabs();
        }

        List<GameObject> passTwoGameObjects = new List<GameObject>();


        // This was one (not 100% working) attempt to get prefabs as EntityPrefabs (skipping this for now)

        //protected Entity ConvertPrefab(GameObject gameobject, ref EntityManager em) {
        //    Entity entity = em.CreateEntity();

        //    foreach (Component comp in gameobject.GetComponents(typeof(Component))) {
        //        if (comp is ICreatePrefabTestData createTestData) {
        //            Debug.Log($"[{gameobject.name}]: convert to EntityPrefab [{comp.GetType()}]");
        //            createTestData.CreateTestData(ref em, ref entity);
        //        } else {
        //            Debug.Log($"[{gameobject.name}]: DON'T convert to EntityPrefab [{comp.GetType()}]");
        //        }
        //        if (comp is ICreatePrefabTestDataAfterCreate) {
        //            passTwoGameObjects.Add(gameobject);
        //        }
        //    }

        //    return entity;
        //}

		//protected void LoadPrefabs(string prefabLookup = "Assets/Prefabs/TestPrefabs.asset") {
  //          passTwoGameObjects.Clear();
		//	var prefabs = AssetDatabase.LoadAssetAtPath<TestDataPrefabs>(prefabLookup);
  //          Assert.IsTrue(prefabs != null);

  //          // first pass
  //          foreach (BuildingComponentAuthoring buildingAuthoring in prefabs.buildingPrefabs) {
  //              GameObject prefab = buildingAuthoring.gameObject;

  //              BuildingType buildingType = buildingAuthoring.buildingType;

  //              Entity prefabEntity = ConvertPrefab(prefab,ref entityManager);

  //              Mgr.assetManager.RegisterBuildingEntityPrefab(buildingType, prefabEntity);

  //              goEntityLookup[prefab] = prefabEntity;

  //          }
  //          // yeah, very verbose
  //          foreach (ResourceComponentAuthoring resourceAuthoring in prefabs.resourcePrefabs) {
  //              GameObject prefab = resourceAuthoring.gameObject;

  //              ResourcePrefabType resourcePrefType = resourceAuthoring.resourcePrefabType;

  //              Entity prefabEntity = ConvertPrefab(prefab, ref entityManager);

  //              Mgr.assetManager.RegisterResourceEntityPrefab(resourcePrefType, prefabEntity);

  //              goEntityLookup[prefab] = prefabEntity;

  //              if (prefab is ICreatePrefabTestDataAfterCreate) {
  //                  passTwoGameObjects.Add(prefab);
  //              }
  //          }

  //          // TODO: Settlers

  //          // second pass
  //          foreach (GameObject prefab in passTwoGameObjects) {
  //              Entity entity = goEntityLookup[prefab];

  //              foreach (Component comp in prefab.GetComponents(typeof(Component))) {
  //                  if (comp is ICreatePrefabTestDataAfterCreate createTestDataAfterCreate) {
  //                      Debug.Log($"[{prefab.name}]: convert to EntityPrefab [{comp.GetType()}]");
  //                      createTestDataAfterCreate.CreateTestDataAfter(ref entityManager, ref entity, goEntityLookup);
  //                  } else {
  //                      Debug.Log($"[{prefab.name}]: DON'T convert to EntityPrefab [{comp.GetType()}]");
  //                  }
  //              }
  //          }

  //      }

        public virtual List<IManager> GetInitializationManagers() {
            return new List<IManager>() {
                Config.Instance,
                Mgr.assetManager,
                Mgr.dataManager,
                Mgr.buildingManager,
                Mgr.jobManager,
                Mgr.settlerManager,
                Mgr.animationManager
            };
        }

        protected void Tick(float dt=0) {
            _minicbMain.Update(dt);
        }

        [TearDown]
        public virtual void TearDown() {
            if (World != null && World.IsCreated) {
                // Note that World.Dispose() already completes all jobs. But some tests may leave tests running when
                // they return, but we can't safely run an internal consistency check with jobs running, so we
                // explicitly complete them here as well.
                World.EntityManager.CompleteAllTrackedJobs();

                // TODO(DOTS-9429): We should not need to explicitly destroy all systems here.
                // World.Dispose() already handles this. However, we currently need to destroy all systems before
                // calling CheckInternalConsistency, or else some tests trigger false positives (due to EntityQuery
                // filters holding references to shared component values, etc.).
                // We can't safely destroy all systems while jobs are running, so this call must come after the
                // CompleteAllTrackedJobs() call above.
                World.DestroyAllSystemsAndLogException(out bool errorsWhileDestroyingSystems);
                Assert.IsFalse(errorsWhileDestroyingSystems,
                    "One or more exceptions were thrown while destroying systems during test teardown; consult the log for details.");

                m_ManagerDebug.CheckInternalConsistency();

                World.Dispose();
                World = null;

                World.DefaultGameObjectInjectionWorld = m_PreviousWorld;
                m_PreviousWorld = null;
                entityManager = default;
            }

            JobsUtility.JobDebuggerEnabled = JobsDebuggerWasEnabled;
            //JobsUtility.ClearSystemIds();

            PlayerLoop.SetPlayerLoop(m_PreviousPlayerLoop);


        }

        public void AssertSameChunk(Entity e0, Entity e1) {
            Assert.AreEqual(entityManager.GetChunk(e0), entityManager.GetChunk(e1));
        }

        public void AssetHasChangeVersion<T>(Entity e, uint version) where T :
#if UNITY_DISABLE_MANAGED_COMPONENTS
        struct,
#endif
        IComponentData {
            var type = entityManager.GetComponentTypeHandle<T>(true);
            var chunk = entityManager.GetChunk(e);
            Assert.AreEqual(version, chunk.GetChangeVersion(ref type));
            Assert.IsFalse(chunk.DidChange(ref type, version));
            Assert.IsTrue(chunk.DidChange(ref type, version - 1));
        }

    }
}
