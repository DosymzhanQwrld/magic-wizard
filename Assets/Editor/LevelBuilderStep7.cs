using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class LevelBuilderStep7
{
    const string CharactersFolder = "Assets/Art/Characters";
    const string GeneratedFolder = "Assets/Art/Generated";
    const float WizardPixelsPerUnit = 60f;
    const float MonsterPixelsPerUnit = 40f;

    [MenuItem("Magic Wizard/Step 7 - Sprites and Lava")]
    public static void Step7()
    {
        if (!LevelBuilder.CheckScene()) return;

        AssetDatabase.Refresh();
        ImportFolder("Wizard", WizardPixelsPerUnit);
        ImportFolder("Goblin", MonsterPixelsPerUnit);
        ImportFolder("Skeleton", MonsterPixelsPerUnit);
        ImportFolder("FlyingEye", MonsterPixelsPerUnit);

        ApplyPlayer();
        ApplyGoblins();
        ApplyFlyingEyes();
        ApplyArchers();
        ApplyHearts();
        MakeLava();

        LevelBuilder.Finish("Step 7 done: sprites, hearts and lava are ready.");
    }

    static void ApplyPlayer()
    {
        var controller = Object.FindFirstObjectByType<PlayerController>();
        if (controller == null) { Debug.LogWarning("No Player in the scene."); return; }
        GameObject go = controller.gameObject;

        Sprite[] idle = Frames("Wizard", "Idle", 6);
        Sprite[] run = Frames("Wizard", "Run", 8);
        Sprite[] air = Frames("Wizard", "Jump", 2);
        if (idle.Length == 0) { Debug.LogWarning("Wizard sprites not found."); return; }

        Vector2 size = new Vector2(0.6f, 1.43f);
        SetVisual(go, idle[0]);
        go.GetComponent<BoxCollider2D>().size = size;
        Vector3 p = go.transform.position;
        go.transform.position = new Vector3(p.x, size.y * 0.5f + 0.02f, 0f);
        LevelBuilder.Set(controller, "groundCheckOffset", size.y * 0.5f);
        SetVector2(controller, "groundCheckSize", new Vector2(size.x * 0.9f, 0.1f));
        AddFlipbook(go, idle, run, air);
    }

    static void ApplyGoblins()
    {
        Sprite[] idle = Frames("Goblin", "Idle", 4);
        Sprite[] run = Frames("Goblin", "Run", 8);
        if (idle.Length == 0) { Debug.LogWarning("Goblin sprites not found."); return; }

        Vector2 size = new Vector2(0.6f, 0.9f);
        int n = 1;
        foreach (GoblinEnemy enemy in SortedByX(Object.FindObjectsByType<GoblinEnemy>(FindObjectsSortMode.None)))
        {
            GameObject go = enemy.gameObject;
            RenamePoints(go.name, "Goblin " + n);
            go.name = "Goblin " + n++;
            SetVisual(go, idle[0]);
            go.GetComponent<BoxCollider2D>().size = size;
            Vector3 p = go.transform.position;
            go.transform.position = new Vector3(p.x, size.y * 0.5f + 0.02f, 0f);
            PlaceLabel(go, size.y);
            AddFlipbook(go, idle, run, null);
        }
    }

    static void ApplyFlyingEyes()
    {
        Sprite[] flight = Frames("FlyingEye", "Flight", 8);
        if (flight.Length == 0) { Debug.LogWarning("Flying eye sprites not found."); return; }

        int n = 1;
        foreach (FlyingEyeEnemy enemy in SortedByX(Object.FindObjectsByType<FlyingEyeEnemy>(FindObjectsSortMode.None)))
        {
            GameObject go = enemy.gameObject;
            go.name = "Flying Eye " + n++;
            SetVisual(go, flight[0]);
            go.GetComponent<CircleCollider2D>().radius = 0.35f;
            PlaceLabel(go, 0.8f);
            AddFlipbook(go, flight, null, null);
        }
    }

    static void ApplyArchers()
    {
        Sprite[] idle = Frames("Skeleton", "Idle", 4);
        if (idle.Length == 0) { Debug.LogWarning("Skeleton sprites not found."); return; }

        Vector2 size = new Vector2(0.6f, 1.27f);
        const float ledgeTop = 2.75f;
        foreach (ArcherEnemy enemy in Object.FindObjectsByType<ArcherEnemy>(FindObjectsSortMode.None))
        {
            GameObject go = enemy.gameObject;
            SetVisual(go, idle[0]);
            go.GetComponent<BoxCollider2D>().size = size;
            Vector3 p = go.transform.position;
            go.transform.position = new Vector3(p.x, ledgeTop + size.y * 0.5f + 0.02f, 0f);
            PlaceLabel(go, size.y);
            AddFlipbook(go, idle, null, null);
        }
    }

    static void ApplyHearts()
    {
        Sprite heart = MakeHeartSprite();
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null) return;
        Transform hearts = LevelBuilder.FindDeep(canvas.transform, "Hearts");
        if (hearts == null) return;
        foreach (Transform child in hearts)
        {
            if (!child.TryGetComponent(out Image image)) continue;
            image.sprite = heart;
            image.color = Color.white;
            image.preserveAspect = true;
            EditorUtility.SetDirty(image);
        }
    }

    static void MakeLava()
    {
        LevelBuilder.Remove("Lava");
        GameObject level = GameObject.Find("Level");
        Sprite square = LevelBuilder.MakeSprite("Square", false);
        GameObject lava = LevelBuilder.Sprite2D("Lava", square, LevelBuilder.Hex("F06B1F"),
            new Vector2(21f, -1.1f), new Vector2(2f, 1.6f), level != null ? level.transform : null, 2);
        lava.AddComponent<BoxCollider2D>().isTrigger = true;
        lava.AddComponent<Hazard>();
    }

    static void SetVisual(GameObject go, Sprite sprite)
    {
        go.transform.localScale = Vector3.one;
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = Color.white;
        sr.flipX = false;
    }

    static void PlaceLabel(GameObject go, float height)
    {
        Transform label = go.transform.Find("State Label");
        if (label == null) return;
        label.localScale = Vector3.one;
        label.localPosition = new Vector3(0f, height * 0.5f + 0.4f, 0f);
    }

    static void RenamePoints(string oldName, string newName)
    {
        foreach (string suffix in new[] { " Point A", " Point B" })
        {
            GameObject point = GameObject.Find(oldName + suffix);
            if (point != null) point.name = newName + suffix;
        }
    }

    static void AddFlipbook(GameObject go, Sprite[] idle, Sprite[] move, Sprite[] air)
    {
        if (!go.TryGetComponent(out SpriteFlipbook flipbook)) flipbook = go.AddComponent<SpriteFlipbook>();
        LevelBuilderStep2.SetArray(flipbook, "idleFrames", idle);
        LevelBuilderStep2.SetArray(flipbook, "moveFrames", move ?? new Sprite[0]);
        LevelBuilderStep2.SetArray(flipbook, "airFrames", air ?? new Sprite[0]);
    }

    static T[] SortedByX<T>(T[] items) where T : Component
    {
        System.Array.Sort(items, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        return items;
    }

    static Sprite[] Frames(string character, string clip, int count)
    {
        var list = new System.Collections.Generic.List<Sprite>();
        for (int i = 0; i < count; i++)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{CharactersFolder}/{character}/{clip}_{i}.png");
            if (sprite != null) list.Add(sprite);
        }
        return list.ToArray();
    }

    static void ImportFolder(string character, float pixelsPerUnit)
    {
        string folder = $"{CharactersFolder}/{character}";
        if (!Directory.Exists(folder)) return;
        foreach (string file in Directory.GetFiles(folder, "*.png"))
            ImportSprite(file.Replace('\\', '/'), pixelsPerUnit);
    }

    static void ImportSprite(string path, float pixelsPerUnit)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        if (importer.textureType == TextureImporterType.Sprite
            && importer.spriteImportMode == SpriteImportMode.Single
            && Mathf.Approximately(importer.spritePixelsPerUnit, pixelsPerUnit)
            && importer.filterMode == FilterMode.Point) return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = pixelsPerUnit;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
    }

    static Sprite MakeHeartSprite()
    {
        string path = $"{GeneratedFolder}/Heart.png";
        if (!File.Exists(path))
        {
            string[] rows =
            {
                ".##...##.",
                "#rr#.#rr#",
                "#wrr#rrr#",
                "#rrrrrrr#",
                ".#rrrrr#.",
                "..#rrr#..",
                "...#r#...",
                "....#...."
            };
            const int scale = 8;
            int width = rows[0].Length * scale;
            int height = rows.Length * scale;
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color outline = LevelBuilder.Hex("4B1528");
            Color red = LevelBuilder.Hex("E8433A");
            Color light = LevelBuilder.Hex("F7A8A2");
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                char c = rows[rows.Length - 1 - y / scale][x / scale];
                Color color = Color.clear;
                if (c == '#') color = outline;
                else if (c == 'r') color = red;
                else if (c == 'w') color = light;
                tex.SetPixel(x, y, color);
            }
            Directory.CreateDirectory(GeneratedFolder);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
        }
        ImportSprite(path, 64f);
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static void SetVector2(Object target, string field, Vector2 value)
    {
        var so = new SerializedObject(target);
        SerializedProperty p = so.FindProperty(field);
        if (p == null) { Debug.LogError($"{target.GetType().Name}.{field} not found"); return; }
        p.vector2Value = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
