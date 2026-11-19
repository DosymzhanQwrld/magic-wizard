using UnityEngine;

[CreateAssetMenu(fileName = "BossStats", menuName = "Magic Wizard/Boss Stats")]
public class BossStats : ScriptableObject
{
    [Header("Health")]
    public int maxHealth;
    public int scoreValue;

    [Header("Flight")]
    public float flySpeed;
    public float hoverAmplitude;
    public float hoverSpeed;

    [Header("Attack")]
    public float shootInterval;
    public float windUpTime;
    public float projectileSpeed;
    public int projectileDamage;

    [Header("Phase 2")]
    [Range(0f, 1f)] public float phase2HealthFraction;
    public float phase2SpeedMultiplier;
    public int phase2ProjectileCount;
    public float phase2SpreadAngle;
}
