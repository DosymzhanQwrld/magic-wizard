using UnityEngine;

[CreateAssetMenu(fileName = "GoblinStats", menuName = "Magic Wizard/Goblin Stats")]
public class GoblinStats : ScriptableObject
{
    [Header("Health")]
    public int maxHealth;
    public int scoreValue;

    [Header("Vision")]
    public float viewDistance;
    public float viewAngle;

    [Header("Movement")]
    public float patrolSpeed;
    public float chaseSpeed;
    public float attackRange;
    public float loseSightTime;

    [Header("Attack")]
    public float windUpTime;
    public float lungeSpeed;
    public float lungeTime;
    public float cooldownTime;
    public int damage;
    public float hitRadius;
}
