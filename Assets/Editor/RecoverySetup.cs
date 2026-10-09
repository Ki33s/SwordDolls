using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
public static class RecoverySetup
{
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        int missing = scene.GetRootGameObjects().Sum(root => root.GetComponentsInChildren<Transform>(true).Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)));
        Debug.Log("RECOVERY_SCENE missingScripts=" + missing);
        // QuickOutline reads vertices/normals and writes UV4 at runtime.
        // Keep CPU mesh data only for models used by outlined game prefabs.
        var meshPaths = new System.Collections.Generic.HashSet<string>();
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" }))
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (!prefab) continue;
            foreach (var outline in prefab.GetComponentsInChildren<Outline>(true))
            {
                foreach (var filter in outline.GetComponentsInChildren<MeshFilter>(true))
                    if (filter.sharedMesh) meshPaths.Add(AssetDatabase.GetAssetPath(filter.sharedMesh));
                foreach (var skin in outline.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    if (skin.sharedMesh) meshPaths.Add(AssetDatabase.GetAssetPath(skin.sharedMesh));
            }
        }
        int readableModels = 0;
        foreach (var path in meshPaths)
            if (AssetImporter.GetAtPath(path) is ModelImporter model && !model.isReadable)
            { model.isReadable = true; model.SaveAndReimport(); readableModels++; }
        Debug.Log("RECOVERY_OUTLINE readableModels=" + readableModels);
        var pipeline = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>("Assets/Settings/Ultra_PipelineAsset.asset");
        if (!pipeline) throw new Exception("Original Ultra URP asset missing");
        GraphicsSettings.defaultRenderPipeline = pipeline;
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true) };
        PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
        PlayerSettings.runInBackground = true;
        AssetDatabase.SaveAssets();
        Directory.CreateDirectory("Builds/Windows");
        var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Builds/Windows/SwordHeroDolls.exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);
        Debug.Log("RECOVERY_BUILD " + report.summary.result);
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) throw new Exception("Recovery build failed");
    }
}
