using TMPro;
using UnityEditor;
using UnityEngine;

public static class LevelBuilderStep3
{
    [MenuItem("Magic Wizard/Step 3 - Slime Enemy")]
    public static void Step3()
    {
        if (!LevelBuilder.CheckScene()) return;
        if (!RequirePlayer()) return;

        Transform enemies = EnemiesRoot();
        MakeSlime("Slime 1", new Vector2(12f, 0.35f), 8f, 17f, enemies);
        MakeSlime("Slime 2", new Vector2(53f, 0.35f), 49f, 57f, enemies);

        LevelBuilder.Finish("Step 3 done: slimes are ready.");
    }

    static void MakeSlime(string name, Vector2 pos, float leftX, float rightX, Transform parent)
    {
        LevelBuilder.Remove(name);
        LevelBuilder.Remove(name + " Point A");
        LevelBuilder.Remove(name + " Point B");

        int groundMask = 1 << LevelBuilder.EnsureLayer("Ground");
        Sprite circle = LevelBuilder.MakeSprite("Circle", true);

        GameObject go = LevelBuilder.Sprite2D(name, circle, LevelBuilder.Hex("97C459"), pos, new Vector2(1f, 0.7f), parent, 4);
        go.layer = LevelBuilder.EnsureLayer("Enemy");
        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        go.AddComponent<BoxCollider2D>();

        var vision = go.AddComponent<EnemyVision>();
        LevelBuilder.Set(vision, "obstacleMask", groundMask);
        LevelBuilder.Set(vision, "viewDistance", 6f);
        LevelBuilder.Set(vision, "viewAngle", 120f);

        var hp = go.AddComponent<EnemyHealth>();
        LevelBuilder.Set(hp, "maxHealth", 2);
        LevelBuilder.Set(hp, "scoreValue", 100);

        Transform a = new GameObject(name + " Point A").transform;
        a.SetParent(parent);
        a.position = new Vector3(leftX, pos.y, 0f);
        Transform b = new GameObject(name + " Point B").transform;
        b.SetParent(parent);
        b.position = new Vector3(rightX, pos.y, 0f);

        var slime = go.AddComponent<SlimeEnemy>();
        LevelBuilder.Set(slime, "pointA", a);
        LevelBuilder.Set(slime, "pointB", b);
        LevelBuilder.Set(slime, "stateLabel", Label(go, "Patrol"));
    }

    internal static bool RequirePlayer()
    {
        if (Object.FindFirstObjectByType<PlayerHealth>() != null) return true;
        EditorUtility.DisplayDialog("Magic Wizard", "No Player with PlayerHealth in the scene. Run Step 1 and Step 2 first.", "OK");
        return false;
    }

    internal static Transform EnemiesRoot()
    {
        GameObject root = GameObject.Find("Enemies");
        if (root == null) root = new GameObject("Enemies");
        return root.transform;
    }

    internal static TMP_Text Label(GameObject owner, string text)
    {
        var go = new GameObject("State Label");
        go.transform.SetParent(owner.transform, false);
        Vector3 s = owner.transform.localScale;
        go.transform.localScale = new Vector3(1f / s.x, 1f / s.y, 1f);
        go.transform.localPosition = new Vector3(0f, (0.5f * s.y + 0.45f) / s.y, 0f);
        var tmp = go.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = 3f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.rectTransform.sizeDelta = new Vector2(3f, 0.6f);
        tmp.sortingOrder = 10;
        return tmp;
    }
}
