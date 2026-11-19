using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public static class LevelBuilderStep11
{
    const string LevelFolder = "Assets/Data/Levels";
    const string PrefabFolder = "Assets/Prefabs";

    public static void Step11()
    {
        if (!LevelBuilder.CheckScene()) return;

        GameObject level = GameObject.Find("Level");
        var goblinScene = Object.FindFirstObjectByType<GoblinEnemy>();
        var eyeScene = Object.FindFirstObjectByType<FlyingEyeEnemy>();
        var archerScene = Object.FindFirstObjectByType<ArcherEnemy>();
        var portalScene = Object.FindFirstObjectByType<LevelGoal>();
        var lavaScene = Object.FindFirstObjectByType<Hazard>();
        var player = Object.FindFirstObjectByType<PlayerController>();
        var cameraFollow = Object.FindFirstObjectByType<CameraFollow>();

        if (level == null || goblinScene == null || eyeScene == null || archerScene == null
            || portalScene == null || lavaScene == null || player == null)
        {
            EditorUtility.DisplayDialog("Magic Wizard",
                "Step 11 needs the hand-built level from Steps 1-9 (Level, enemies, portal, lava, player). It is already converted or missing.", "OK");
            return;
        }

        Directory.CreateDirectory(LevelFolder);
        Directory.CreateDirectory(PrefabFolder);

        LevelData level1 = SaveLevel("Level_01", ExtractLevel1(level, player, cameraFollow));
        LevelData level2 = SaveLevel("Level_02", BuildLevel2());
        LevelData level3 = SaveLevel("Level_03", BuildLevel3());

        GoblinEnemy goblinPrefab = MakePrefab(goblinScene, "Goblin");
        FlyingEyeEnemy eyePrefab = MakePrefab(eyeScene, "FlyingEye");
        ArcherEnemy archerPrefab = MakePrefab(archerScene, "Archer");
        LevelGoal portalPrefab = MakePrefab(portalScene, "Portal");
        Hazard lavaPrefab = MakePrefab(lavaScene, "Lava");

        Object.DestroyImmediate(level);
        LevelBuilder.Remove("Enemies");
        LevelBuilder.Remove("LevelLoader");

        var loader = new GameObject("LevelLoader").AddComponent<LevelLoader>();
        LevelBuilderStep2.SetArray(loader, "levels", new Object[] { level1, level2, level3 });
        LevelBuilder.Set(loader, "player", player.transform);
        if (cameraFollow != null) LevelBuilder.Set(loader, "cameraFollow", cameraFollow);
        LevelBuilder.Set(loader, "goblinPrefab", goblinPrefab);
        LevelBuilder.Set(loader, "flyingEyePrefab", eyePrefab);
        LevelBuilder.Set(loader, "archerPrefab", archerPrefab);
        LevelBuilder.Set(loader, "portalPrefab", portalPrefab);
        LevelBuilder.Set(loader, "lavaPrefab", lavaPrefab);
        LevelBuilder.Set(loader, "grassSprite", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Environment/GrassStrip.png"));
        LevelBuilder.Set(loader, "dirtSprite", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Environment/Dirt.png"));

        AddTimeText();

        Selection.activeObject = level1;
        EditorGUIUtility.PingObject(level1);
        LevelBuilder.Finish("Step 11 done: Level1 scene now builds Level_01, Level_02 or Level_03 from data.");
    }

    static LevelData ExtractLevel1(GameObject level, PlayerController player, CameraFollow cameraFollow)
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.levelNumber = 1;
        data.displayName = "LEVEL 1";
        data.timeLimit = 120f;
        data.playerSpawn = Feet(player);

        int groundLayer = LayerMask.NameToLayer("Ground");
        foreach (Transform t in level.transform)
        {
            if (t.gameObject.layer != groundLayer || !t.TryGetComponent(out BoxCollider2D box) || box.isTrigger) continue;
            Vector2 size = Vector2.Scale(box.size, t.lossyScale);
            data.blocks.Add(new BlockData { center = (Vector2)t.position + box.offset, size = size });
        }

        foreach (Hazard lava in Object.FindObjectsByType<Hazard>(FindObjectsSortMode.None))
            data.lava.Add(new BlockData { center = lava.transform.position, size = lava.transform.lossyScale });

        foreach (GoblinEnemy g in Object.FindObjectsByType<GoblinEnemy>(FindObjectsSortMode.None))
        {
            var so = new SerializedObject(g);
            var a = so.FindProperty("pointA").objectReferenceValue as Transform;
            var b = so.FindProperty("pointB").objectReferenceValue as Transform;
            float left = a != null ? a.position.x : g.transform.position.x - 3f;
            float right = b != null ? b.position.x : g.transform.position.x + 3f;
            data.enemies.Add(new EnemySpawn { type = EnemyType.Goblin, position = Feet(g), patrolLeft = Mathf.Min(left, right), patrolRight = Mathf.Max(left, right) });
        }

        foreach (FlyingEyeEnemy e in Object.FindObjectsByType<FlyingEyeEnemy>(FindObjectsSortMode.None))
            data.enemies.Add(new EnemySpawn { type = EnemyType.FlyingEye, position = e.transform.position });

        foreach (ArcherEnemy a in Object.FindObjectsByType<ArcherEnemy>(FindObjectsSortMode.None))
        {
            int facing = new SerializedObject(a).FindProperty("startFacing").intValue;
            data.enemies.Add(new EnemySpawn { type = EnemyType.Archer, position = Feet(a), facing = facing == 0 ? -1 : facing });
        }

        data.enemies.Sort((x, y) => x.position.x.CompareTo(y.position.x));

        var portal = Object.FindFirstObjectByType<LevelGoal>();
        data.goalPosition = Feet(portal);

        if (cameraFollow != null)
        {
            var so = new SerializedObject(cameraFollow);
            data.cameraMinX = so.FindProperty("minX").floatValue;
            data.cameraMaxX = so.FindProperty("maxX").floatValue;
        }
        return data;
    }

    internal static LevelData BuildLevel2()
    {
        var d = ScriptableObject.CreateInstance<LevelData>();
        d.levelNumber = 2;
        d.displayName = "LEVEL 2";
        d.timeLimit = 150f;
        d.playerSpawn = new Vector2(0f, 0f);
        d.goalPosition = new Vector2(70f, 0f);
        d.cameraMinX = 4f;
        d.cameraMaxX = 64f;

        Floor(d, -4f, 14f);
        Floor(d, 17f, 34f);
        Floor(d, 36.5f, 72f);
        Lava(d, 14f, 17f);
        Lava(d, 34f, 36.5f);
        Wall(d, -4.5f);
        Wall(d, 72.5f);
        Block(d, 20f, 0.75f, 1f, 1.5f);
        Block(d, 24f, 2.5f, 4f, 0.5f);
        Block(d, 45f, 1.5f, 2.5f, 0.5f);
        Block(d, 55f, 0.75f, 1f, 1.5f);
        Block(d, 61f, 2.5f, 3f, 0.5f);

        Goblin(d, 8f, 3f, 13f);
        Archer(d, 22.4f, 2.75f);
        Eye(d, 30f, 4.5f);
        Goblin(d, 42f, 38f, 50f);
        Eye(d, 52f, 4.8f);
        Archer(d, 59.9f, 2.75f);
        Goblin(d, 64f, 57f, 68f);
        return d;
    }

    static LevelData BuildLevel3()
    {
        var d = ScriptableObject.CreateInstance<LevelData>();
        d.levelNumber = 3;
        d.displayName = "LEVEL 3";
        d.timeLimit = 180f;
        d.playerSpawn = new Vector2(0f, 0f);
        d.goalPosition = new Vector2(90f, 0f);
        d.cameraMinX = 4f;
        d.cameraMaxX = 84f;

        Floor(d, -4f, 12f);
        Floor(d, 15f, 30f);
        Floor(d, 33f, 50f);
        Floor(d, 52.5f, 92f);
        Lava(d, 12f, 15f);
        Lava(d, 30f, 33f);
        Lava(d, 50f, 52.5f);
        Wall(d, -4.5f);
        Wall(d, 92.5f);
        Block(d, 22f, 0.75f, 1f, 1.5f);
        Block(d, 27f, 2.5f, 3f, 0.5f);
        Block(d, 41f, 0.75f, 1f, 1.5f);
        Block(d, 46f, 2.5f, 3f, 0.5f);
        Block(d, 66f, 1.5f, 2.5f, 0.5f);
        Block(d, 74f, 0.75f, 1f, 1.5f);
        Block(d, 80f, 2.5f, 3f, 0.5f);

        Goblin(d, 6f, 2f, 11f);
        Eye(d, 18f, 4.5f);
        Archer(d, 25.9f, 2.75f);
        Goblin(d, 37f, 34f, 40f);
        Eye(d, 36f, 4.8f);
        Archer(d, 44.9f, 2.75f);
        Goblin(d, 60f, 55f, 65f);
        Eye(d, 63f, 4.5f);
        Goblin(d, 70f, 67f, 73f);
        Archer(d, 78.9f, 2.75f);
        Eye(d, 84f, 4.8f);
        return d;
    }

    internal static void Floor(LevelData d, float left, float right) => Block(d, (left + right) * 0.5f, -0.5f, right - left, 1f);
    internal static void Wall(LevelData d, float x) => Block(d, x, 5f, 1f, 12f);
    internal static void Block(LevelData d, float x, float y, float w, float h) => d.blocks.Add(new BlockData { center = new Vector2(x, y), size = new Vector2(w, h) });
    internal static void Lava(LevelData d, float left, float right) => d.lava.Add(new BlockData { center = new Vector2((left + right) * 0.5f, -1.1f), size = new Vector2(right - left, 1.6f) });
    internal static void Goblin(LevelData d, float x, float left, float right) => d.enemies.Add(new EnemySpawn { type = EnemyType.Goblin, position = new Vector2(x, 0f), patrolLeft = left, patrolRight = right });
    internal static void Eye(LevelData d, float x, float y) => d.enemies.Add(new EnemySpawn { type = EnemyType.FlyingEye, position = new Vector2(x, y) });
    internal static void Archer(LevelData d, float x, float ledgeTop) => d.enemies.Add(new EnemySpawn { type = EnemyType.Archer, position = new Vector2(x, ledgeTop), facing = -1 });

    internal static LevelData SaveLevel(string name, LevelData data)
    {
        string path = $"{LevelFolder}/{name}.asset";
        var existing = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        if (existing != null)
        {
            EditorUtility.CopySerialized(data, existing);
            EditorUtility.SetDirty(existing);
            Object.DestroyImmediate(data);
            return existing;
        }
        AssetDatabase.CreateAsset(data, path);
        return data;
    }

    static T MakePrefab<T>(T sceneObject, string name) where T : Component
    {
        GameObject copy = Object.Instantiate(sceneObject.gameObject);
        copy.name = name;
        copy.transform.SetParent(null);
        copy.transform.position = Vector3.zero;

        if (copy.TryGetComponent(out GoblinEnemy goblin))
        {
            var so = new SerializedObject(goblin);
            so.FindProperty("pointA").objectReferenceValue = null;
            so.FindProperty("pointB").objectReferenceValue = null;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(copy, $"{PrefabFolder}/{name}.prefab");
        Object.DestroyImmediate(copy);
        return prefab.GetComponent<T>();
    }

    static Vector2 Feet(Component c)
    {
        Vector3 p = c.transform.position;
        float scaleY = Mathf.Abs(c.transform.lossyScale.y);
        float half = 0.5f * scaleY;
        if (c.TryGetComponent(out BoxCollider2D box)) half = box.size.y * scaleY * 0.5f;
        else if (c.TryGetComponent(out CircleCollider2D circle)) half = circle.radius * scaleY;
        return new Vector2(p.x, Mathf.Round((p.y - half) * 100f) / 100f);
    }

    static void AddTimeText()
    {
        var session = Object.FindFirstObjectByType<GameSession>();
        GameObject canvas = GameObject.Find("Canvas");
        if (session == null || canvas == null) return;

        Transform time = LevelBuilder.FindDeep(canvas.transform, "TimeText");
        if (time == null)
        {
            Transform safeArea = LevelBuilder.FindDeep(canvas.transform, "SafeArea");
            if (safeArea == null) safeArea = canvas.transform;
            TextMeshProUGUI t = LevelBuilderStep2.NewText("TimeText", safeArea, "TIME 2:00", 40f);
            RectTransform rt = t.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(40f, -140f);
            rt.sizeDelta = new Vector2(320f, 60f);
            t.alignment = TextAlignmentOptions.Left;
            time = t.transform;
        }
        LevelBuilder.Set(session, "timeText", time.GetComponent<TMP_Text>());

        Transform tries = LevelBuilder.FindDeep(canvas.transform, "TriesText");
        if (tries != null) ((RectTransform)tries).sizeDelta = new Vector2(600f, 60f);
    }
}
