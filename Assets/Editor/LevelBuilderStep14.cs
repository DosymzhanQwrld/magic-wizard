using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class LevelBuilderStep14
{
    const string Cave = "Assets/Art/Environment/Cave";
    const string Volcano = "Assets/Art/Environment/Volcano";

    public static void Step14()
    {
        if (EditorSceneManager.GetActiveScene().isDirty && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        AssetDatabase.Refresh();

        Sprite caveTop = Import($"{Cave}/CaveTop.png", 32f, FilterMode.Point);
        Sprite caveFill = Import($"{Cave}/CaveFill.png", 32f, FilterMode.Point);
        Sprite caveBg = Import($"{Cave}/CaveBackground.png", 16f, FilterMode.Point);
        Sprite volcanoTop = Import($"{Volcano}/VolcanoTop.png", 128f, FilterMode.Bilinear);
        Sprite volcanoFill = Import($"{Volcano}/VolcanoFill.png", 128f, FilterMode.Bilinear);
        Sprite volcanoBg = Import($"{Volcano}/VolcanoBackground.png", 40f, FilterMode.Bilinear);

        if (caveTop == null || volcanoTop == null)
        {
            EditorUtility.DisplayDialog("Magic Wizard", "Cave or volcano images not found in Assets/Art/Environment.", "OK");
            return;
        }

        SetLook("Assets/Data/Levels/Level_02.asset", caveTop, caveFill, 0.5f, caveBg, LevelBuilder.Hex("1A0E14"));
        SetLook("Assets/Data/Levels/Level_03.asset", volcanoTop, volcanoFill, volcanoTop.rect.height / volcanoTop.pixelsPerUnit, volcanoBg, LevelBuilder.Hex("2A1840"));
        AssetDatabase.SaveAssets();

        for (int i = 1; i <= 3; i++)
        {
            string path = $"Assets/Scenes/Level{i}.unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null) continue;
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            var loader = Object.FindFirstObjectByType<LevelLoader>();
            Camera cam = Camera.main;
            if (loader != null && cam != null)
            {
                Transform bg = cam.transform.Find("Background");
                if (bg != null) LevelBuilder.Set(loader, "cameraBackground", bg);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        EditorSceneManager.OpenScene("Assets/Scenes/Level2.unity", OpenSceneMode.Single);
        Debug.Log("Magic Wizard: Step 14 done. Level 2 is a cave, Level 3 is a volcano.");
    }

    static void SetLook(string path, Sprite top, Sprite fill, float topHeight, Sprite background, Color sky)
    {
        var data = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        if (data == null) { Debug.LogWarning("Not found: " + path); return; }
        data.groundTopSprite = top;
        data.groundFillSprite = fill;
        data.groundTopHeight = Mathf.Round(topHeight * 100f) / 100f;
        data.backgroundSprite = background;
        data.skyColor = sky;
        EditorUtility.SetDirty(data);
    }

    static Sprite Import(string path, float pixelsPerUnit, FilterMode filter)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return null;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = pixelsPerUnit;
        importer.filterMode = filter;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Repeat;
        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
