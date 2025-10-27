using Unity.Entities;

namespace Manager {
    /// <summary>
    /// Settler-Specific functions
    /// </summary>
    public class SettlerManager : ISettlerManager {
        private EntityManager entityManager;
        private IDataManager dataManager;

        public SettlerManager() {
        }

        public void Init() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            dataManager = Mgr.dataManager;
        }

        public void Dispose() {
        }

    }
}