using NUnit.Framework;
using System.Collections;
using System.Linq;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestHelper {
    public const string DEFAULT_SCENE = "Main";

    //MiniCBMain runtime;

    //protected void InitMiniCB() {
    //    runtime = new MiniCBMain();
    //    runtime.Init();
    //}

    /// <summary>
    /// coroutine to load a scene
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    protected IEnumerator LoadScene(string sceneName) {
        // Load the scene asynchronously (make sure it's in Build Settings)
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone) {
            yield return null;
        }

        Assert.IsTrue(SceneManager.GetActiveScene().name == sceneName);
    }

    /// <summary>
    /// coroutine to load the default game-scene
    /// </summary>
    /// <returns></returns>
    protected IEnumerator LoadDefaultScene() {
        yield return LoadScene(DEFAULT_SCENE);
        yield return new WaitForSeconds(2); // wait for the subscene to finish (TODO: implement are more robust way)
    }
}