using UnityEngine;

[CreateAssetMenu(fileName = "ArcherStats", menuName = "Magic Wizard/Archer Stats")]
public class ArcherStats : ScriptableObject
{
    [Header("Health")]
    public int maxHealth;
    public int scoreValue;

    [Header("Vision")]
    public float viewDistance;
    public float viewAngle;
    public float turnInterval;

    [Header("Attack")]
    public float windUpTime;
    public float cooldownTime;
    public float arrowSpeed;
    public int arrowDamage;
}
