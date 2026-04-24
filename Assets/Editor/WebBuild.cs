// Assets/Editor/WebBuild.cs

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Build.Reporting;

public static class WebBuild
{
    public static void Build()
    {
        string mode = GetArg("-webMode", "webgpu-webgl");
        string output = GetArg("-output", $"build/{mode}");

        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.WebGL,
            BuildTarget.WebGL
        );

        GraphicsDeviceType[] apis = mode switch
        {
            "webgpu-only" => new[]
            {
                GraphicsDeviceType.WebGPU
            },

            "webgpu-webgl" => new[]
            {
                GraphicsDeviceType.WebGPU,
                GraphicsDeviceType.OpenGLES3
            },

            _ => throw new Exception($"Unknown -webMode: {mode}")
        };

        // WebGPU is not enabled by Auto Graphics API.
        // Disable auto mode and explicitly set the Web graphics APIs.
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, apis);

        Directory.CreateDirectory(output);

        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            throw new Exception("No enabled scenes found in EditorBuildSettings.");
        }

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = output,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Build failed: {report.summary.result}");
        }

        Debug.Log($"Build succeeded: {mode} -> {output}");
    }

    private static string GetArg(string name, string defaultValue)
    {
        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == name)
            {
                return args[i + 1];
            }
        }

        return defaultValue;
    }
}
