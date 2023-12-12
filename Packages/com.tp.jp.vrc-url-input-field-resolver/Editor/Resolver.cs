using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

namespace TpLab.VrcUrlInputFieldResolver.Editor
{
    public static class Resolver
    {
        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            EditorApplicationUtility.ProjectWasLoaded += ProjectWasLoaded;
        }

        static void ProjectWasLoaded()
        {
            // Missing check
            if (!DetectMissingScripts())
            {
                return;
            }

            // var message = "シーン内にMissingScriptが存在します。\nSDKのReloadとプレハブのReimportを実行しますか？";
            // if (!EditorUtility.DisplayDialog("Confirm", message, "Yes", "No")) return;

            // SDK Reload
            Debug.Log("Reload SDK");
            EditorApplication.ExecuteMenuItem("VRChat SDK/Reload SDK");
        
            // Reimport
            Debug.Log("Reimport");
            var paths = AssetDatabase
                .FindAssets("t:prefab")
                .Where(DetectMissingScriptsForPrefab);
            foreach (var path in paths)
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
            }
        }
        
        static bool DetectMissingScripts()
        {
            var scene = EditorSceneManager.GetActiveScene();
            return scene.GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<Component>(true))
                .Any(x => x == null);
        }

        static bool DetectMissingScriptsForPrefab(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (obj == null) return false;
            return obj.GetComponentsInChildren<Component>(true)
                .Any(x => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(x.gameObject) > 0);
        }
    }

    [InitializeOnLoad]
    public static class EditorApplicationUtility
    {
        const BindingFlags BINDING_ATTR = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
        static readonly FieldInfo m_info = typeof(EditorApplication).GetField("projectWasLoaded", BINDING_ATTR);

        public static UnityAction ProjectWasLoaded
        {
            get { return m_info.GetValue(null) as UnityAction; }
            set
            {
                var functions = m_info.GetValue(null) as UnityAction;
                functions += value;
                m_info.SetValue(null, functions);
            }
        }
    }
}
