using System.IO;
using UnityEditor;
using UnityEngine;

public static class StatsAssets
{
    public const string Folder = "Assets/Data/Stats";

    public static PlayerStats Player()
    {
        return GetOrCreate<PlayerStats>("PlayerStats", s =>
        {
            s.moveSpeed = 6f;
            s.jumpVelocity = 12f;
            s.maxHearts = 3;
            s.invincibleTime = 1f;
            s.knockback = new Vector2(5f, 6f);
            s.knockbackLockTime = 0.25f;
            s.fireballCooldown = 0.5f;
            s.fireballSpeed = 11f;
            s.fireballDamage = 1;
        });
    }

    public static GoblinStats Goblin()
    {
        return GetOrCreate<GoblinStats>("GoblinStats", s =>
        {
            s.maxHealth = 2;
            s.scoreValue = 100;
            s.viewDistance = 6f;
            s.viewAngle = 120f;
            s.patrolSpeed = 1.5f;
            s.chaseSpeed = 3f;
            s.attackRange = 1.8f;
            s.loseSightTime = 1.5f;
            s.windUpTime = 0.5f;
            s.lungeSpeed = 7f;
            s.lungeTime = 0.25f;
            s.cooldownTime = 1.2f;
            s.damage = 1;
            s.hitRadius = 0.8f;
        });
    }

    public static FlyingEyeStats FlyingEye()
    {
        return GetOrCreate<FlyingEyeStats>("FlyingEyeStats", s =>
        {
            s.maxHealth = 1;
            s.scoreValue = 150;
            s.hoverAmplitude = 0.3f;
            s.hoverSpeed = 2f;
            s.detectRange = 5.5f;
            s.windUpTime = 0.6f;
            s.shakeAmount = 0.08f;
            s.diveSpeed = 9f;
            s.maxDiveTime = 1.2f;
            s.returnSpeed = 4f;
            s.cooldownTime = 1.5f;
            s.damage = 1;
            s.hitRadius = 0.6f;
        });
    }

    public static ArcherStats Archer()
    {
        return GetOrCreate<ArcherStats>("ArcherStats", s =>
        {
            s.maxHealth = 3;
            s.scoreValue = 200;
            s.viewDistance = 9f;
            s.viewAngle = 140f;
            s.turnInterval = 3f;
            s.windUpTime = 0.6f;
            s.cooldownTime = 1.6f;
            s.arrowSpeed = 8f;
            s.arrowDamage = 1;
        });
    }

    static T GetOrCreate<T>(string name, System.Action<T> fill) where T : ScriptableObject
    {
        string path = $"{Folder}/{name}.asset";
        var existing = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existing != null) return existing;
        Directory.CreateDirectory(Folder);
        var asset = ScriptableObject.CreateInstance<T>();
        fill(asset);
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        return asset;
    }
}
