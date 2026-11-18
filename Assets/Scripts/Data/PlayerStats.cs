using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Magic Wizard/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpVelocity;

    [Header("Health")]
    public int maxHearts;
    public float invincibleTime;
    public Vector2 knockback;
    public float knockbackLockTime;

    [Header("Fireball")]
    public float fireballCooldown;
    public float fireballSpeed;
    public int fireballDamage;
}
