using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class LevelBuilderStep8
{
    const string EnvironmentFolder = "Assets/Art/Environment";
    const string GeneratedFolder = "Assets/Art/Generated";
    const float TilePixelsPerUnit = 32f;
    const float BackgroundPixelsPerUnit = 14f;
    const float BackgroundWidth = 24f;
    const float TileSize = 0.5f;

    [MenuItem("Magic Wizard/Step 8 - Ground and Background")]
    public static void Step8()
    {
        if (!LevelBuilder.CheckScene()) return;

        AssetDatabase.Refresh();
        Sprite grass = ImportSprite($"{EnvironmentFolder}/GrassStrip.png", TilePixelsPerUnit, true, Vector4.zero);
        Sprite dirt = ImportSprite($"{EnvironmentFolder}/Dirt.png", TilePixelsPerUnit, true, Vector4.zero);
        Sprite night = ImportSprite($"{EnvironmentFolder}/Night.png", BackgroundPixelsPerUnit, true, Vector4.zero);
        Sprite farForest = ImportSprite($"{EnvironmentFolder}/FarForest.png", BackgroundPixelsPerUnit, true, Vector4.zero);
        Sprite darkTree = ImportSprite($"{EnvironmentFolder}/DarkTree.png", BackgroundPixelsPerUnit, true, Vector4.zero);
        if (grass == null || dirt == null || night == null)
        {
            EditorUtility.DisplayDialog("Magic Wizard", "Environment sprites not found in Assets/Art/Environment.", "OK");
            return;
        }

        ApplyGround(grass, dirt);
        ApplyBackground(night, farForest, darkTree);

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null) StyleUi(canvas.transform, new[] { "PauseButton", "ResumeButton", "RestartButton", "MenuButton", "Window" });

        LevelBuilder.Finish("Step 8 done: ground, background and pause menu look are ready.");
    }

    [MenuItem("Magic Wizard/Step 9 - Main Menu Look")]
    public static void Step9()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "MainMenu")
        {
            EditorUtility.DisplayDialog("Magic Wizard", "Open the MainMenu scene first.", "OK");
            return;
        }

        AssetDatabase.Refresh();
        Sprite background = ImportSprite($"{EnvironmentFolder}/MenuBackground.png", 100f, false, Vector4.zero);
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null || background == null)
        {
            EditorUtility.DisplayDialog("Magic Wizard", "Canvas or MenuBackground.png not found.", "OK");
            return;
        }

        Transform bg = LevelBuilder.FindDeep(canvas.transform, "Background");
        if (bg != null && bg.TryGetComponent(out Image image))
        {
            image.sprite = background;
            image.color = Color.white;
            image.type = Image.Type.Simple;
            if (!bg.TryGetComponent(out AspectRatioFitter fitter)) fitter = bg.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = 336f / 192f;
            EditorUtility.SetDirty(image);
        }

        Transform title = LevelBuilder.FindDeep(canvas.transform, "Title");
        if (title != null && title.TryGetComponent(out TMP_Text titleText))
        {
            titleText.color = LevelBuilder.Hex("FFD9A0");
            titleText.fontStyle = FontStyles.Bold;
            EditorUtility.SetDirty(titleText);
        }

        StyleUi(canvas.transform, new[] { "PlayButton", "QuitButton" });
        LevelBuilder.Finish("Step 9 done: main menu background and buttons are ready.");
    }

    static void ApplyGround(Sprite grass, Sprite dirt)
    {
        GameObject level = GameObject.Find("Level");
        if (level == null) { Debug.LogWarning("No Level object. Run Step 1 first."); return; }
        int groundLayer = LevelBuilder.EnsureLayer("Ground");

        foreach (Transform block in level.transform)
        {
            if (block.gameObject.layer != groundLayer) continue;
            if (!block.TryGetComponent(out BoxCollider2D box)) continue;

            Vector2 size = Vector2.Scale(box.size, block.localScale);
            Vector3 pos = block.position;
            if (block.name == "Step") { size = new Vector2(2.5f, 0.5f); pos.y = 1.55f; }
            if (block.name == "Cover Wall") { size = new Vector2(1f, 1.5f); pos.y = 0.75f; }
            size.x = Mathf.Max(TileSize, Mathf.Round(size.x / TileSize) * TileSize);
            size.y = Mathf.Max(TileSize, Mathf.Round(size.y / TileSize) * TileSize);

            var old = new List<GameObject>();
            foreach (Transform child in block) old.Add(child.gameObject);
            foreach (GameObject child in old) Object.DestroyImmediate(child);

            block.localScale = Vector3.one;
            block.position = pos;
            box.size = size;
            if (block.TryGetComponent(out SpriteRenderer rootRenderer)) rootRenderer.enabled = false;

            Tiled("Dirt", dirt, block, Vector2.zero, size, 0);
            Tiled("Top", grass, block, new Vector2(0f, size.y * 0.5f - TileSize * 0.5f), new Vector2(size.x, TileSize), 1);
        }
    }

    static void Tiled(string name, Sprite sprite, Transform parent, Vector2 localPos, Vector2 size, int order)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.tileMode = SpriteTileMode.Continuous;
        sr.size = size;
        sr.sortingOrder = order;
    }

    static void ApplyBackground(Sprite night, Sprite farForest, Sprite darkTree)
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        cam.backgroundColor = LevelBuilder.Hex("2A1B33");

        Transform old = cam.transform.Find("Background");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        Transform root = new GameObject("Background").transform;
        root.SetParent(cam.transform, false);
        root.localPosition = new Vector3(0f, 0f, 10f);

        float height = BackgroundWidth * 192f / 336f;

        var sky = new GameObject("Night Sky").AddComponent<SpriteRenderer>();
        sky.transform.SetParent(root, false);
        sky.sprite = night;
        sky.sortingOrder = -30;

        Layer("Far Forest", farForest, root, height, 0.05f, -20);
        Layer("Dark Trees", darkTree, root, height, 0.15f, -10);
    }

    static void Layer(string name, Sprite sprite, Transform root, float height, float factor, int order)
    {
        if (sprite == null) return;
        var go = new GameObject(name);
        go.transform.SetParent(root, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.tileMode = SpriteTileMode.Continuous;
        sr.size = new Vector2(BackgroundWidth * 4f, height);
        sr.sortingOrder = order;
        var parallax = go.AddComponent<ParallaxLayer>();
        LevelBuilder.Set(parallax, "factor", factor);
        LevelBuilder.Set(parallax, "tileWidth", BackgroundWidth);
    }

    static void StyleUi(Transform canvas, string[] names)
    {
        Sprite panel = MakePanelSprite();
        foreach (string name in names)
        {
            Transform t = LevelBuilder.FindDeep(canvas, name);
            if (t == null || !t.TryGetComponent(out Image image)) continue;
            image.sprite = panel;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            EditorUtility.SetDirty(image);

            if (name == "Window") continue;
            foreach (TMP_Text text in t.GetComponentsInChildren<TMP_Text>(true))
            {
                text.color = LevelBuilder.Hex("FFE9C4");
                text.fontStyle = FontStyles.Bold;
                EditorUtility.SetDirty(text);
            }
        }
    }

    static Sprite MakePanelSprite()
    {
        string path = $"{GeneratedFolder}/Panel.png";
        if (!File.Exists(path))
        {
            const int size = 16;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color outline = LevelBuilder.Hex("1E1528");
            Color border = LevelBuilder.Hex("C8663C");
            Color fill = LevelBuilder.Hex("4A2F5C");
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                int edge = Mathf.Min(Mathf.Min(x, y), Mathf.Min(size - 1 - x, size - 1 - y));
                bool corner = (x == 0 || x == size - 1) && (y == 0 || y == size - 1);
                Color c = fill;
                if (corner) c = Color.clear;
                else if (edge == 0) c = outline;
                else if (edge <= 2) c = border;
                tex.SetPixel(x, y, c);
            }
            Directory.CreateDirectory(GeneratedFolder);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
        }
        return ImportSprite(path, 16f, false, new Vector4(5f, 5f, 5f, 5f));
    }

    static Sprite ImportSprite(string path, float pixelsPerUnit, bool fullRect, Vector4 border)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return null;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = pixelsPerUnit;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.spriteBorder = border;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = fullRect ? SpriteMeshType.FullRect : SpriteMeshType.Tight;
        importer.SetTextureSettings(settings);
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
