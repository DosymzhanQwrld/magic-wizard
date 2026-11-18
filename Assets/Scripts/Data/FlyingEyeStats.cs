using UnityEngine;

[CreateAssetMenu(fileName = "FlyingEyeStats", menuName = "Magic Wizard/Flying Eye Stats")]
public class FlyingEyeStats : ScriptableObject
{
    [Header("Health")]
    public int maxHealth;
    public int scoreValue;

    [Header("Hover")]
    public float hoverAmplitude;
    public float hoverSpeed;
    public float detectRange;

    [Header("Attack")]
    public float windUpTime;
    public float shakeAmount;
    public float diveSpeed;
    public float maxDiveTime;
    public float returnSpeed;
    public float cooldownTime;
    public int damage;
    public float hitRadius;
}
