using UnityEngine;
using UnityEditor;
using System.Runtime.CompilerServices;
using System;
using UnityEngine.Assertions;

public class PrefabMigrationHelper : EditorWindow {
    /// <summary>
    /// Iterate over all prefabs
    /// </summary>
    /// <param name="conversionLogic"></param>
    /// <param name="outputPrefabInfo"></param>
    /// <returns></returns>
    protected static int MigratePrefab(Func<GameObject, bool> conversionLogic, bool outputPrefabInfo=true) {
        Assert.IsNotNull(conversionLogic, "Call without converionLogic doesn't make sense! If it does, remove assert!");

        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

        int migratedCount = 0;

        foreach (string guid in prefabGUIDs) {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);

            // Load prefab as GameObject (not instantiated in scene)
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
                continue;

            bool migrationSuccessful = conversionLogic(prefab);

            if (migrationSuccessful) {
                // Save changes to prefab asset
                PrefabUtility.SavePrefabAsset(prefab);

                migratedCount++;
                if (outputPrefabInfo) {
                    Debug.Log($"Migrated prefab: {prefabPath}");
                }
            }
        }
        return migratedCount;
    }

    /// <summary>
    /// Iterate over all prefabs that have a specific component attached
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="conversionLogic"></param>
    /// <param name="outputPrefabInfo"></param>
    /// <returns></returns>
    protected static int MigratePrefabFrom<T>(Func<GameObject, T, bool> conversionLogic, bool outputPrefabInfo=true) where T : MonoBehaviour {
        return MigratePrefab((prefab) => {
            var conversionComponent = prefab.GetComponent<T>();
            if (conversionComponent == null) {
                return false;
            }

            return conversionLogic(prefab, conversionComponent);
        },outputPrefabInfo);
    }
}

