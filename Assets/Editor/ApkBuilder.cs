using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class ApkBuilder
{
    const string OutputFolder = "Builds";
    const string ApkName = "MagicWizard.apk";

    [MenuItem("Magic Wizard/Build Android APK")]
    public static void Build()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            if (!EditorUtility.DisplayDialog("Magic Wizard", "Switch the platform to Android first? This can take a few minutes.", "Switch", "Cancel"))
                return;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }

        PlayerSettings.productName = "Magic Wizard";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.dossymzhan.magicwizard");
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        EditorUserBuildSettings.buildAppBundle = false;

        Directory.CreateDirectory(OutputFolder);
        string path = Path.Combine(OutputFolder, ApkName);

        var options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Level1.unity" },
            locationPathName = path,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            string full = Path.GetFullPath(path);
            Debug.Log($"Magic Wizard: APK built, {summary.totalSize / (1024 * 1024)} MB: {full}");
            EditorUtility.RevealInFinder(full);
        }
        else
        {
            Debug.LogError($"Magic Wizard: build {summary.result} with {summary.totalErrors} errors. See the Console.");
        }
    }
}
