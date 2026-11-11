using System.IO;
using UnityEditor;
using UnityEngine;

public static class LevelBuilderStep5
{
    const string ArtFolder = "Assets/Art/Generated";

    [MenuItem("Magic Wizard/Step 5 - Archer Enemy")]
    public static void Step5()
    {
        if (!LevelBuilder.CheckScene()) return;
        if (!LevelBuilderStep3.RequirePlayer()) return;

        int groundMask = 1 << LevelBuilder.EnsureLayer("Ground");
        Sprite square = LevelBuilder.MakeSprite("Square", false);
        Projectile arrow = LevelBuilderStep2.MakeProjectilePrefab("Arrow", square, LevelBuilder.Hex("B4B2A9"),
            new Vector2(0.7f, 0.12f), 8f, true, groundMask);
        Material lineMaterial = MakeLineMaterial();

        Transform enemies = LevelBuilderStep3.EnemiesRoot();
        MakeArcher("Archer 1", new Vector2(41.2f, 3.35f), enemies, groundMask, square, arrow, lineMaterial);
        MakeArcher("Archer 2", new Vector2(58.5f, 3.35f), enemies, groundMask, square, arrow, lineMaterial);

        LevelBuilder.Finish("Step 5 done: archers are ready.");
    }

    static void MakeArcher(string name, Vector2 pos, Transform parent, int groundMask, Sprite square,
        Projectile arrow, Material lineMaterial)
    {
        LevelBuilder.Remove(name);

        GameObject go = LevelBuilder.Sprite2D(name, square, LevelBuilder.Hex("F1EFE8"), pos, new Vector2(0.7f, 1.2f), parent, 4);
        go.layer = LevelBuilder.EnsureLayer("Enemy");
        go.AddComponent<BoxCollider2D>();

        var vision = go.AddComponent<EnemyVision>();
        LevelBuilder.Set(vision, "obstacleMask", groundMask);
        LevelBuilder.Set(vision, "viewDistance", 9f);
        LevelBuilder.Set(vision, "viewAngle", 140f);

        var hp = go.AddComponent<EnemyHealth>();
        LevelBuilder.Set(hp, "maxHealth", 3);
        LevelBuilder.Set(hp, "scoreValue", 200);

        var lineGo = new GameObject("Aim Line");
        lineGo.transform.SetParent(go.transform, false);
        var line = lineGo.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = 2;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.sharedMaterial = lineMaterial;
        line.startColor = LevelBuilder.Hex("E24B4A");
        line.endColor = LevelBuilder.Hex("E24B4A");
        line.sortingOrder = 8;
        line.enabled = false;

        var archer = go.AddComponent<ArcherEnemy>();
        LevelBuilder.Set(archer, "startFacing", -1);
        LevelBuilder.Set(archer, "arrowPrefab", arrow);
        LevelBuilder.Set(archer, "aimLine", line);
        LevelBuilder.Set(archer, "stateLabel", LevelBuilderStep3.Label(go, "Idle"));
    }

    static Material MakeLineMaterial()
    {
        string path = $"{ArtFolder}/AimLine.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;
        Directory.CreateDirectory(ArtFolder);
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        material = new Material(shader);
        AssetDatabase.CreateAsset(material, path);
        return material;
    }
}
