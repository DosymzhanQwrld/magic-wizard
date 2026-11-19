using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class LevelBuilderStep12
{
    const string BossFolder = "Assets/Art/Characters/Boss";
    const float BossPixelsPerUnit = 18f;
    const float ProjectilePixelsPerUnit = 32f;

    public static void Step12()
    {
        if (!LevelBuilder.CheckScene()) return;

        var loader = Object.FindFirstObjectByType<LevelLoader>();
        if (loader == null)
        {
            EditorUtility.DisplayDialog("Magic Wizard", "No LevelLoader in the scene. Run Step 11 first.", "OK");
            return;
        }

        AssetDatabase.Refresh();
        Sprite[] fly = Frames("Fly", 4);
        Sprite[] attack = Frames("Attack", 8);
        Sprite[] death = Frames("Death", 7);
        Sprite projectileSprite = Import($"{BossFolder}/Projectile.png", ProjectilePixelsPerUnit);
        if (fly.Length == 0 || projectileSprite == null)
        {
            EditorUtility.DisplayDialog("Magic Wizard", "Boss sprites not found in " + BossFolder, "OK");
            return;
        }

        int groundMask = 1 << LevelBuilder.EnsureLayer("Ground");
        Projectile bossShot = MakeBossProjectile(projectileSprite, groundMask);
        BossEnemy bossPrefab = MakeBossPrefab(fly, attack, death, bossShot);
        BossHealthBar bar = MakeHealthBar();

        LevelData level2 = LevelBuilderStep11.SaveLevel("Level_02", LevelBuilderStep11.BuildLevel2());
        LevelData level3 = LevelBuilderStep11.SaveLevel("Level_03", BuildBossLevel());

        var so = new SerializedObject(loader);
        SerializedProperty levels = so.FindProperty("levels");
        if (levels.arraySize < 3) levels.arraySize = 3;
        levels.GetArrayElementAtIndex(1).objectReferenceValue = level2;
        levels.GetArrayElementAtIndex(2).objectReferenceValue = level3;
        so.FindProperty("bossPrefab").objectReferenceValue = bossPrefab;
        so.FindProperty("bossHealthBar").objectReferenceValue = bar;
        so.ApplyModifiedPropertiesWithoutUndo();

        Selection.activeObject = level3;
        EditorGUIUtility.PingObject(level3);
        LevelBuilder.Finish("Step 12 done: Level 2 is harder, Level 3 ends with the Flying Demon boss.");
    }

    static LevelData BuildBossLevel()
    {
        var d = ScriptableObject.CreateInstance<LevelData>();
        d.levelNumber = 3;
        d.displayName = "LEVEL 3";
        d.timeLimit = 180f;
        d.playerSpawn = new Vector2(0f, 0f);
        d.goalPosition = new Vector2(68f, 0f);
        d.cameraMinX = 4f;
        d.cameraMaxX = 61f;

        LevelBuilderStep11.Floor(d, -4f, 20f);
        LevelBuilderStep11.Floor(d, 22.5f, 70f);
        LevelBuilderStep11.Lava(d, 20f, 22.5f);
        LevelBuilderStep11.Wall(d, -4.5f);
        LevelBuilderStep11.Wall(d, 70.5f);
        LevelBuilderStep11.Block(d, 29f, 0.75f, 1f, 1.5f);
        LevelBuilderStep11.Block(d, 34f, 2.5f, 3f, 0.5f);

        LevelBuilderStep11.Goblin(d, 10f, 5f, 18f);
        LevelBuilderStep11.Eye(d, 26f, 4.5f);
        LevelBuilderStep11.Archer(d, 32.9f, 2.75f);

        d.hasBoss = true;
        d.bossPosition = new Vector2(62f, 2.3f);
        d.bossLeftX = 53f;
        d.bossRightX = 66f;
        d.bossActivateX = 46f;
        return d;
    }

    static BossEnemy MakeBossPrefab(Sprite[] fly, Sprite[] attack, Sprite[] death, Projectile shot)
    {
        var go = new GameObject("Boss");
        go.layer = LevelBuilder.EnsureLayer("Enemy");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = fly[0];
        sr.sortingOrder = 4;
        go.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        var box = go.AddComponent<BoxCollider2D>();
        box.size = new Vector2(2.4f, 3f);
        box.offset = new Vector2(0f, -0.2f);

        var health = go.AddComponent<EnemyHealth>();
        LevelBuilder.Set(health, "destroyOnDeath", false);

        var boss = go.AddComponent<BossEnemy>();
        LevelBuilder.Set(boss, "stats", StatsAssets.Boss());
        LevelBuilder.Set(boss, "projectilePrefab", shot);
        LevelBuilderStep2.SetArray(boss, "flyFrames", fly);
        LevelBuilderStep2.SetArray(boss, "attackFrames", attack);
        LevelBuilderStep2.SetArray(boss, "deathFrames", death);

        TMP_Text label = LevelBuilderStep3.Label(go, "Sleep");
        label.transform.localPosition = new Vector3(0f, 2.3f, 0f);
        LevelBuilder.Set(boss, "stateLabel", label);

        Directory.CreateDirectory("Assets/Prefabs");
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/Prefabs/Boss.prefab");
        Object.DestroyImmediate(go);
        return prefab.GetComponent<BossEnemy>();
    }

    static Projectile MakeBossProjectile(Sprite sprite, int groundMask)
    {
        var go = new GameObject("BossFireball");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 6;
        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.3f;
        col.offset = new Vector2(0.3f, 0f);
        var p = go.AddComponent<Projectile>();
        LevelBuilder.Set(p, "hitsPlayer", true);
        LevelBuilder.Set(p, "groundMask", groundMask);
        LevelBuilder.Set(p, "lifeTime", 4f);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/Prefabs/BossFireball.prefab");
        Object.DestroyImmediate(go);
        return prefab.GetComponent<Projectile>();
    }

    static BossHealthBar MakeHealthBar()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null) return null;

        Transform old = LevelBuilder.FindDeep(canvas.transform, "BossHealthBar");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        Transform safeArea = LevelBuilder.FindDeep(canvas.transform, "SafeArea");
        if (safeArea == null) safeArea = canvas.transform;
        Sprite square = LevelBuilder.MakeSprite("Square", false);

        var root = new GameObject("BossHealthBar", typeof(RectTransform), typeof(Image));
        root.transform.SetParent(safeArea, false);
        var rt = (RectTransform)root.transform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -205f);
        rt.sizeDelta = new Vector2(800f, 64f);
        var bg = root.GetComponent<Image>();
        bg.sprite = square;
        bg.color = new Color(0.1f, 0.05f, 0.12f, 0.85f);
        bg.raycastTarget = false;

        var fillGo = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillGo.transform.SetParent(root.transform, false);
        var frt = (RectTransform)fillGo.transform;
        frt.anchorMin = Vector2.zero;
        frt.anchorMax = Vector2.one;
        frt.offsetMin = new Vector2(6f, 6f);
        frt.offsetMax = new Vector2(-6f, -6f);
        var fill = fillGo.GetComponent<Image>();
        fill.sprite = square;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.fillAmount = 1f;
        fill.color = LevelBuilder.Hex("E24B4A");
        fill.raycastTarget = false;

        TextMeshProUGUI label = LevelBuilderStep2.NewText("Label", root.transform, "FLYING DEMON", 34f);
        RectTransform lrt = label.rectTransform;
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;
        label.fontStyle = FontStyles.Bold;

        var bar = root.AddComponent<BossHealthBar>();
        LevelBuilder.Set(bar, "fill", fill);
        LevelBuilder.Set(bar, "label", label);
        root.SetActive(false);
        return bar;
    }

    static Sprite[] Frames(string clip, int count)
    {
        var list = new List<Sprite>();
        for (int i = 0; i < count; i++)
        {
            Sprite s = Import($"{BossFolder}/{clip}_{i}.png", BossPixelsPerUnit);
            if (s != null) list.Add(s);
        }
        return list.ToArray();
    }

    static Sprite Import(string path, float pixelsPerUnit)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return null;
        if (importer.textureType != TextureImporterType.Sprite
            || importer.spriteImportMode != SpriteImportMode.Single
            || !Mathf.Approximately(importer.spritePixelsPerUnit, pixelsPerUnit)
            || importer.filterMode != FilterMode.Point)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
