using System;

namespace Manager {
    public interface IManager : IDisposable {
        /// <summary>
        /// Initialize the manager (once on startup)
        /// </summary>
        void Init();
    }

    public interface IManagerUpdateable {
        void Update(float dt);
    }
}