using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDKBase.Editor.Source.Helpers;

namespace TpLab.VrcUrlInputFieldResolver.Editor
{
    public static class Resolver
    {
        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        /// <summary>
        /// シーンを開いた際に呼ばれるイベント。
        /// </summary>
        /// <param name="scene">開いたシーン</param>
        /// <param name="mode">シーンを開くモード</param>
        static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            // Missing check
            if (!DetectMissingScripts(scene))
            {
                return;
            }
            Log("Resolving...");

            // Reload SDK
            Log("Reload SDK");
            var targetPaths = FindFoldersWithMissingScripts();
            ReloadUtil.ReloadSDK(false);

            // Reimport
            Log("Reimport");
            foreach (var path in targetPaths)
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ImportRecursive);
            }

            // Reopen scene
            EditorCoroutine.Start(ReopenScene(scene));
        }
        
        /// <summary>
        /// プレハブ内の欠落しているスクリプトを検出し、対応するフォルダの一覧
        /// </summary>
        /// <returns></returns>
        static string[] FindFoldersWithMissingScripts()
        {
            return AssetDatabase
                .FindAssets("t:prefab")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(DetectMissingScriptsForPrefab)
                .Select(x =>
                {
                    var pathArray = x.Split('/');
                    return pathArray.Length > 1 ? $"{pathArray[0]}/{pathArray[1]}" : "";
                })
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// シーンを再度開く。
        /// </summary>
        /// <param name="scene">シーン</param>
        /// <returns>コレクション参照IF</returns>
        static IEnumerator ReopenScene(Scene scene)
        {
            yield return new WaitForSeconds(3f);
            const int retryCount = 3;
            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    EditorSceneManager.OpenScene(scene.path);
                    Log("Resolved.");
                    break;
                }
                catch (Exception)
                {
                    // ignored
                }
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// シーン内から欠落しているスクリプトを検出する。
        /// </summary>
        /// <param name="scene">対象シーン</param>
        /// <returns>MissingScriptがある場合はtrue、それ以外はfalse</returns>
        static bool DetectMissingScripts(Scene scene)
        {
            return scene.GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<Component>(true))
                .Any(x => x == null);
        }

        /// <summary>
        /// プレハブ内から欠落しているスクリプトを検出する。
        /// </summary>
        /// <param name="path">プレハブのパス</param>
        /// <returns>MissingScriptがある場合はtrue、それ以外はfalse</returns>
        static bool DetectMissingScriptsForPrefab(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (obj == null) return false;
            return obj.GetComponentsInChildren<Transform>(true)
                .Any(x => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(x.gameObject) > 0);
        }

        /// <summary>
        /// ログを出力する。
        /// </summary>
        /// <param name="message">メッセージ</param>
        static void Log(string message)
        {
            Debug.Log($"[<color=#4EC9B0>VRCUrlInputFieldResolver</color>]: {message}");
        }
    }
}
