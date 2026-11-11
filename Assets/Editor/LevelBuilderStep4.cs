using UnityEditor;
using UnityEngine;

public static class LevelBuilderStep4
{
    [MenuItem("Magic Wizard/Step 4 - Bat Enemy")]
    public static void Step4()
    {
        if (!LevelBuilder.CheckScene()) return;
        if (!LevelBuilderStep3.RequirePlayer()) return;

        Transform enemies = LevelBuilderStep3.EnemiesRoot();
        MakeBat("Bat 1", new Vector2(29f, 4.5f), enemies);
        MakeBat("Bat 2", new Vector2(55f, 4.8f), enemies);

        LevelBuilder.Finish("Step 4 done: bats are ready.");
    }

    static void MakeBat(string name, Vector2 pos, Transform parent)
    {
        LevelBuilder.Remove(name);

        Sprite circle = LevelBuilder.MakeSprite("Circle", true);
        GameObject go = LevelBuilder.Sprite2D(name, circle, LevelBuilder.Hex("D85A30"), pos, new Vector2(0.8f, 0.5f), parent, 4);
        go.layer = LevelBuilder.EnsureLayer("Enemy");
        go.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        go.AddComponent<CircleCollider2D>();

        var hp = go.AddComponent<EnemyHealth>();
        LevelBuilder.Set(hp, "maxHealth", 1);
        LevelBuilder.Set(hp, "scoreValue", 150);

        var bat = go.AddComponent<BatEnemy>();
        LevelBuilder.Set(bat, "groundMask", 1 << LevelBuilder.EnsureLayer("Ground"));
        LevelBuilder.Set(bat, "stateLabel", LevelBuilderStep3.Label(go, "Hover"));
    }
}
