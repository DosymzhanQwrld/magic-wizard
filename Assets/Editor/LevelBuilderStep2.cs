using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class LevelBuilderStep2
{
    const string PrefabFolder = "Assets/Prefabs";

    [MenuItem("Magic Wizard/Step 2 - Fireball and Health")]
    public static void Step2()
    {
        if (!LevelBuilder.CheckScene()) return;

        var controller = Object.FindFirstObjectByType<PlayerController>();
        if (controller == null)
        {
            EditorUtility.DisplayDialog("Magic Wizard", "No Player in the scene. Run Step 1 first.", "OK");
            return;
        }
        GameObject player = controller.gameObject;

        int groundLayer = LevelBuilder.EnsureLayer("Ground");
        int playerLayer = LevelBuilder.EnsureLayer("Player");
        int enemyLayer = LevelBuilder.EnsureLayer("Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);

        Sprite circle = LevelBuilder.MakeSprite("Circle", true);
        Projectile fireball = MakeProjectilePrefab("Fireball", circle, LevelBuilder.Hex("EF9F27"),
            new Vector2(0.4f, 0.4f), 11f, false, 1 << groundLayer);

        if (!player.TryGetComponent(out PlayerHealth health)) health = player.AddComponent<PlayerHealth>();
        if (!player.TryGetComponent(out PlayerShooter shooter)) shooter = player.AddComponent<PlayerShooter>();
        LevelBuilder.Set(shooter, "fireballPrefab", fireball);

        LevelBuilder.Remove("GameSession");
        var session = new GameObject("GameSession").AddComponent<GameSession>();

        WireHud(health, shooter, session);
        LevelBuilder.Finish("Step 2 done: fireball, hearts and restart are ready.");
        Selection.activeGameObject = player;
    }

    static void WireHud(PlayerHealth health, PlayerShooter shooter, GameSession session)
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null) { Debug.LogWarning("No Canvas found."); return; }
        Transform root = canvas.transform;
        Transform safeArea = LevelBuilder.FindDeep(root, "SafeArea");
        if (safeArea == null) safeArea = root;

        Transform hearts = LevelBuilder.FindDeep(root, "Hearts");
        if (hearts != null)
        {
            var images = new Object[hearts.childCount];
            for (int i = 0; i < hearts.childCount; i++) images[i] = hearts.GetChild(i).GetComponent<Image>();
            SetArray(health, "heartImages", images);
        }

        Transform score = LevelBuilder.FindDeep(root, "ScoreText");
        if (score != null) LevelBuilder.Set(session, "scoreText", score.GetComponent<TMP_Text>());

        Transform tries = LevelBuilder.FindDeep(root, "TriesText");
        if (tries == null)
        {
            TextMeshProUGUI t = NewText("TriesText", safeArea, "TRY 1", 40f);
            RectTransform rt = t.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -135f);
            rt.sizeDelta = new Vector2(400f, 60f);
            tries = t.transform;
        }
        LevelBuilder.Set(session, "triesText", tries.GetComponent<TMP_Text>());

        MobileButton fireButton = LevelBuilder.AddMobileButton(LevelBuilder.FindDeep(root, "FireButton"));
        if (fireButton != null) LevelBuilder.Set(shooter, "fireButton", fireButton);
        else Debug.LogWarning("'FireButton' not found in the Canvas.");
    }

    internal static TextMeshProUGUI NewText(string name, Transform parent, string text, float size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.alignment = TextAlignmentOptions.Center;
        t.color = Color.white;
        t.raycastTarget = false;
        return t;
    }

    internal static Projectile MakeProjectilePrefab(string name, Sprite sprite, Color color, Vector2 scale,
        float speed, bool hitsPlayer, int groundMask)
    {
        Directory.CreateDirectory(PrefabFolder);
        GameObject go = LevelBuilder.Sprite2D(name, sprite, color, Vector2.zero, scale, null, 6);
        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        go.AddComponent<BoxCollider2D>().isTrigger = true;
        var p = go.AddComponent<Projectile>();
        LevelBuilder.Set(p, "speed", speed);
        LevelBuilder.Set(p, "hitsPlayer", hitsPlayer);
        LevelBuilder.Set(p, "groundMask", groundMask);
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{PrefabFolder}/{name}.prefab");
        Object.DestroyImmediate(go);
        return prefab.GetComponent<Projectile>();
    }

    internal static void SetArray(Object target, string field, Object[] values)
    {
        var so = new SerializedObject(target);
        SerializedProperty p = so.FindProperty(field);
        p.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++) p.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
