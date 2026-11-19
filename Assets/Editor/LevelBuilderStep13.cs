using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class LevelBuilderStep13
{
    const string Level1Path = "Assets/Scenes/Level1.unity";

    public static void Step13()
    {
        var active = EditorSceneManager.GetActiveScene();
        if (active.isDirty && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        var level1 = EditorSceneManager.OpenScene(Level1Path, OpenSceneMode.Single);
        if (Object.FindFirstObjectByType<LevelLoader>() == null)
        {
            EditorUtility.DisplayDialog("Magic Wizard", "Level1 has no LevelLoader. Run Step 11 and Step 12 first.", "OK");
            return;
        }
        SetSceneLevel(0);
        EditorSceneManager.MarkSceneDirty(level1);
        EditorSceneManager.SaveScene(level1);

        for (int i = 2; i <= 3; i++)
        {
            string path = $"Assets/Scenes/Level{i}.unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null) AssetDatabase.DeleteAsset(path);
            AssetDatabase.CopyAsset(Level1Path, path);

            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            SetSceneLevel(i - 1);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        AddToBuild(new[] { "Assets/Scenes/MainMenu.unity", Level1Path, "Assets/Scenes/Level2.unity", "Assets/Scenes/Level3.unity" });

        EditorSceneManager.OpenScene(Level1Path, OpenSceneMode.Single);
        Debug.Log("Magic Wizard: Step 13 done. Level1, Level2 and Level3 scenes each load their own LevelData.");
    }

    static void SetSceneLevel(int index)
    {
        var loader = Object.FindFirstObjectByType<LevelLoader>();
        var so = new SerializedObject(loader);
        so.FindProperty("sceneLevel").intValue = index;
        so.FindProperty("editorTestLevel").intValue = -1;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void AddToBuild(string[] paths)
    {
        var scenes = new List<EditorBuildSettingsScene>();
        foreach (string p in paths) scenes.Add(new EditorBuildSettingsScene(p, true));
        foreach (var s in EditorBuildSettings.scenes)
            if (!paths.Contains(s.path) && System.IO.File.Exists(s.path)) scenes.Add(new EditorBuildSettingsScene(s.path, false));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
