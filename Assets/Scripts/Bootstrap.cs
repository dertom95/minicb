using UnityEngine;

public class Bootstrap : MonoBehaviour {
    MiniCBMain main;
    public void Awake() {
        main = new MiniCBMain();
        main.Init();
    }

    public void Update() {
        float dt = Time.deltaTime;
        main.Update(dt);
    }
}