using UnityEditor;
using UnityEngine;

public static class LevelBuilderStep4
{
    public static void Step4()
    {
        if (!LevelBuilder.CheckScene()) return;
        if (!LevelBuilderStep3.RequirePlayer()) return;

        Transform enemies = LevelBuilderStep3.EnemiesRoot();
        MakeFlyingEye("Flying Eye 1", new Vector2(29f, 4.5f), enemies);
        MakeFlyingEye("Flying Eye 2", new Vector2(55f, 4.8f), enemies);

        LevelBuilder.Finish("Step 4 done: flying eyes are ready.");
    }

    static void MakeFlyingEye(string name, Vector2 pos, Transform parent)
    {
        LevelBuilder.Remove(name);

        Sprite circle = LevelBuilder.MakeSprite("Circle", true);
        GameObject go = LevelBuilder.Sprite2D(name, circle, LevelBuilder.Hex("D85A30"), pos, new Vector2(0.8f, 0.5f), parent, 4);
        go.layer = LevelBuilder.EnsureLayer("Enemy");
        go.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        go.AddComponent<CircleCollider2D>();

        go.AddComponent<EnemyHealth>();

        var eye = go.AddComponent<FlyingEyeEnemy>();
        LevelBuilder.Set(eye, "stats", StatsAssets.FlyingEye());
        LevelBuilder.Set(eye, "groundMask", 1 << LevelBuilder.EnsureLayer("Ground"));
        LevelBuilder.Set(eye, "stateLabel", LevelBuilderStep3.Label(go, "Hover"));
    }
}
