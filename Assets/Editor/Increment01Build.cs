using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class Increment01Build
{
    public static void Windows() => Build(BuildTarget.StandaloneWindows64, NamedBuildTarget.Standalone,
        ScriptingImplementation.Mono2x, "Build/Increment01/Windows/Voronation.exe");
    public static void WebGL() => Build(BuildTarget.WebGL, NamedBuildTarget.WebGL,
        ScriptingImplementation.IL2CPP, "Build/Increment01/WebGL");

    public static void ReserializeDependencyMetadata()
    {
        string[] dlls = Directory.GetFiles("Assets/Plugins/Demigiant/DOTween", "*.dll", SearchOption.AllDirectories);
        AssetDatabase.ForceReserializeAssets(dlls, ForceReserializeAssetsOptions.ReserializeMetadata);
        AssetDatabase.SaveAssets();
        Debug.Log("Increment01 DOTween metadata updated by Unity; GUIDs preserved.");
    }

    private static void Build(BuildTarget target, NamedBuildTarget namedTarget, ScriptingImplementation backend, string output)
    {
        if (PlayerSettings.GetScriptingBackend(namedTarget) != backend)
            throw new InvalidOperationException("Unexpected scripting backend for " + target);
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Menu.unity", "Assets/Scenes/GameScene.unity" },
            target = target,
            locationPathName = output,
            options = BuildOptions.Development
        });
        string result = target + ": " + report.summary.result + "; errors=" + report.summary.totalErrors +
            "; warnings=" + report.summary.totalWarnings + "; Unity=" + Application.unityVersion + "; backend=" + backend;
        File.WriteAllText("Build/Increment01/build-" + target + ".txt", result);
        Debug.Log("Increment01 build " + result);
        if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException(result);
    }
}