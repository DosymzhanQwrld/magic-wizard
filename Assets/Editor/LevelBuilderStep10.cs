using UnityEditor;
using UnityEngine;

public static class LevelBuilderStep10
{
    [MenuItem("Magic Wizard/Step 10 - Stats Data Assets")]
    public static void Step10()
    {
        if (!LevelBuilder.CheckScene()) return;

        PlayerStats player = StatsAssets.Player();
        GoblinStats goblin = StatsAssets.Goblin();
        FlyingEyeStats eye = StatsAssets.FlyingEye();
        ArcherStats archer = StatsAssets.Archer();

        int count = 0;
        foreach (var c in Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None)) { LevelBuilder.Set(c, "stats", player); count++; }
        foreach (var c in Object.FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None)) { LevelBuilder.Set(c, "stats", player); count++; }
        foreach (var c in Object.FindObjectsByType<PlayerShooter>(FindObjectsSortMode.None)) { LevelBuilder.Set(c, "stats", player); count++; }
        foreach (var c in Object.FindObjectsByType<GoblinEnemy>(FindObjectsSortMode.None)) { LevelBuilder.Set(c, "stats", goblin); count++; }
        foreach (var c in Object.FindObjectsByType<FlyingEyeEnemy>(FindObjectsSortMode.None)) { LevelBuilder.Set(c, "stats", eye); count++; }
        foreach (var c in Object.FindObjectsByType<ArcherEnemy>(FindObjectsSortMode.None)) { LevelBuilder.Set(c, "stats", archer); count++; }

        Selection.activeObject = goblin;
        EditorGUIUtility.PingObject(goblin);
        LevelBuilder.Finish($"Step 10 done: {count} components now read their numbers from Assets/Data/Stats.");
    }
}
