using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class LevelBuilder
{
    const string ArtFolder = "Assets/Art/Generated";

    static readonly Color PlayerColor = Hex("7F77DD");
    static readonly Color GroundColor = Hex("5C3D24");
    static readonly Color GrassColor = Hex("639922");
    static readonly Color SkyColor = Hex("26215C");

    [MenuItem("Magic Wizard/Step 1 - Player and Level")]
    public static void Step1()
    {
        if (!CheckScene()) return;

        int groundLayer = EnsureLayer("Ground");
        int playerLayer = EnsureLayer("Player");
        Sprite square = MakeSprite("Square", false);
        MakeSprite("Circle", true);

        Remove("Level");
        Remove("Player");
        Remove("TestBox");

        Transform level = new GameObject("Level").transform;
        Block("Floor A", new Vector2(8f, -0.5f), new Vector2(24f, 1f), level, square, groundLayer);
        Block("Floor B", new Vector2(43f, -0.5f), new Vector2(42f, 1f), level, square, groundLayer);
        Block("Wall Left", new Vector2(-4.5f, 5f), new Vector2(1f, 12f), level, square, groundLayer);
        Block("Wall Right", new Vector2(64.5f, 5f), new Vector2(1f, 12f), level, square, groundLayer);
        Block("Step", new Vector2(27f, 1.6f), new Vector2(2.5f, 0.4f), level, square, groundLayer);
        Block("Cover Wall", new Vector2(37f, 0.8f), new Vector2(0.8f, 1.6f), level, square, groundLayer);
        Block("Archer Ledge 1", new Vector2(42.5f, 2.5f), new Vector2(3.5f, 0.5f), level, square, groundLayer);
        Block("Archer Ledge 2", new Vector2(59.5f, 2.5f), new Vector2(3f, 0.5f), level, square, groundLayer);

        GameObject player = Sprite2D("Player", square, PlayerColor, new Vector2(0f, 1f), new Vector2(0.8f, 1f), null, 5);
        player.layer = playerLayer;
        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        player.AddComponent<BoxCollider2D>().sharedMaterial = MakeNoFriction();
        var controller = player.AddComponent<PlayerController>();
        Set(controller, "groundMask", 1 << groundLayer);

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(4f, 3.5f, -10f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = SkyColor;
            if (!cam.TryGetComponent(out CameraFollow follow)) follow = cam.gameObject.AddComponent<CameraFollow>();
            Set(follow, "target", player.transform);
        }

        WireMoveControls(controller);
        Finish("Step 1 done: level and player are ready.");
        Selection.activeGameObject = player;
    }

    static void WireMoveControls(PlayerController controller)
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null) { Debug.LogWarning("No Canvas found."); return; }

        Transform joystickRoot = FindDeep(canvas.transform, "Joystick");
        Transform handle = FindDeep(canvas.transform, "Handle");
        Transform jump = FindDeep(canvas.transform, "JumpButton");

        if (joystickRoot != null)
        {
            if (!joystickRoot.TryGetComponent(out MobileJoystick joystick)) joystick = joystickRoot.gameObject.AddComponent<MobileJoystick>();
            if (handle != null)
            {
                Set(joystick, "handle", handle);
                var handleImage = handle.GetComponent<Image>();
                if (handleImage != null) handleImage.raycastTarget = false;
            }
            Set(controller, "joystick", joystick);
        }
        else Debug.LogWarning("'Joystick' not found in the Canvas.");

        MobileButton jumpButton = AddMobileButton(jump);
        if (jumpButton != null) Set(controller, "jumpButton", jumpButton);
        else Debug.LogWarning("'JumpButton' not found in the Canvas.");
    }

    internal static MobileButton AddMobileButton(Transform button)
    {
        if (button == null) return null;
        if (!button.TryGetComponent(out MobileButton b)) b = button.gameObject.AddComponent<MobileButton>();
        return b;
    }

    internal static bool CheckScene()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name == "Level1") return true;
        return EditorUtility.DisplayDialog("Magic Wizard",
            $"The open scene is '{scene.name}', not 'Level1'. Continue anyway?", "Continue", "Cancel");
    }

    internal static void Finish(string message)
    {
        var scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Magic Wizard: " + message);
    }

    internal static void Remove(string name)
    {
        GameObject old = GameObject.Find(name);
        if (old != null) Object.DestroyImmediate(old);
    }

    internal static GameObject Block(string name, Vector2 center, Vector2 size, Transform parent, Sprite square, int groundLayer)
    {
        GameObject go = Sprite2D(name, square, GroundColor, center, size, parent, 0);
        go.layer = groundLayer;
        go.AddComponent<BoxCollider2D>();
        GameObject grass = Sprite2D("Grass", square, GrassColor, Vector2.zero, Vector2.one, go.transform, 1);
        grass.transform.localPosition = new Vector3(0f, 0.5f - 0.06f / size.y, 0f);
        grass.transform.localScale = new Vector3(1f, 0.12f / size.y, 1f);
        return go;
    }

    internal static GameObject Sprite2D(string name, Sprite sprite, Color color, Vector2 pos, Vector2 scale, Transform parent, int order)
    {
        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(scale.x, scale.y, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = order;
        return go;
    }

    internal static Sprite MakeSprite(string name, bool round)
    {
        string path = $"{ArtFolder}/{name}.png";
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(ArtFolder);
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float r = size / 2f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float a = 1f;
                if (round)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
                    a = Mathf.Clamp01(r - d);
                }
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = size;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    internal static PhysicsMaterial2D MakeNoFriction()
    {
        string path = $"{ArtFolder}/NoFriction.physicsMaterial2D";
        var m = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path);
        if (m != null) return m;
        Directory.CreateDirectory(ArtFolder);
        m = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    internal static int EnsureLayer(string name)
    {
        int existing = LayerMask.NameToLayer(name);
        if (existing != -1) return existing;
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        for (int i = 8; i < 32; i++)
        {
            SerializedProperty p = layers.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(p.stringValue))
            {
                p.stringValue = name;
                tagManager.ApplyModifiedPropertiesWithoutUndo();
                return i;
            }
        }
        throw new System.Exception("No free layer slot for " + name);
    }

    internal static void Set(Object target, string field, object value)
    {
        var so = new SerializedObject(target);
        SerializedProperty p = so.FindProperty(field);
        if (p == null) { Debug.LogError($"{target.GetType().Name}.{field} not found"); return; }
        switch (value)
        {
            case int i: p.intValue = i; break;
            case float f: p.floatValue = f; break;
            case bool b: p.boolValue = b; break;
            case Object o: p.objectReferenceValue = o; break;
            default: Debug.LogError($"Unsupported type for {field}"); return;
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    internal static Transform FindDeep(Transform root, string name)
    {
        if (root.name == name) return root;
        foreach (Transform child in root)
        {
            Transform found = FindDeep(child, name);
            if (found != null) return found;
        }
        return null;
    }

    internal static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
