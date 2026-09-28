using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class Increment00Build
{
    private static readonly string[] Scenes =
    {
        "Assets/Scenes/Menu.unity",
        "Assets/Scenes/GameScene.unity"
    };

    [MenuItem("Voronation/Build/Windows Development")]
    public static void Windows()
    {
        Build(BuildTarget.StandaloneWindows64, NamedBuildTarget.Standalone,
            ScriptingImplementation.Mono2x, "Build/Increment00/Windows/Voronation.exe");
    }

    [MenuItem("Voronation/Build/WebGL Development")]
    public static void WebGL()
    {
        Build(BuildTarget.WebGL, NamedBuildTarget.WebGL,
            ScriptingImplementation.IL2CPP, "Build/Increment00/WebGL");
    }

    private static void Build(BuildTarget target, NamedBuildTarget namedTarget,
        ScriptingImplementation requiredBackend, string relativeOutput)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        foreach (string scene in Scenes)
        {
            if (!File.Exists(Path.Combine(projectRoot, scene)))
                throw new FileNotFoundException("Required build scene is missing", scene);
        }

        ScriptingImplementation backend = PlayerSettings.GetScriptingBackend(namedTarget);
        if (backend != requiredBackend)
            throw new InvalidOperationException(target + " requires " + requiredBackend +
                "; current scripting backend is " + backend);

        string output = Path.GetFullPath(Path.Combine(projectRoot, relativeOutput));
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        Debug.Log("Increment 00 build: Unity " + Application.unityVersion +
            ", target=" + target +
            ", backend=" + backend +
            ", API=" + PlayerSettings.GetApiCompatibilityLevel(namedTarget) +
            ", output=" + output);

        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = output,
            target = target,
            options = BuildOptions.Development
        });

        Debug.Log("Increment 00 build result: " + report.summary.result +
            ", errors=" + report.summary.totalErrors +
            ", warnings=" + report.summary.totalWarnings +
            ", size=" + report.summary.totalSize + " bytes");
        if (report.summary.result != BuildResult.Succeeded)
            throw new InvalidOperationException("Increment 00 build failed for " + target);
    }
}
