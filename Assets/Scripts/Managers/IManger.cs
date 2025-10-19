using System;

namespace Manager {
    public interface IManager : IDisposable {
        void Init();
    }

    public interface IManagerUpdateable {
        void Update(float dt);
    }
}