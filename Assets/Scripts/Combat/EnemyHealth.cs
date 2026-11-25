using System;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float blinkTime = 0.15f;
    [SerializeField] bool destroyOnDeath = true;
    [SerializeField, Range(0f, 1f)] float heartDropChance = 0.5f;

    public bool IsDead { get; private set; }
    public int Max { get; private set; }
    public float Fraction => Max > 0 ? (float)health / Max : 0f;
    public float SoundPitch { get; set; } = 1f;

    public event Action Damaged;
    public event Action Died;

    int health;
    int scoreValue;
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Setup(int maxHealth, int score)
    {
        Max = maxHealth;
        health = maxHealth;
        scoreValue = score;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;
        health = Mathf.Max(0, health - damage);
        Damaged?.Invoke();
        if (health == 0)
        {
            IsDead = true;
            if (GameSession.Instance != null) GameSession.Instance.AddScore(scoreValue);
            Sfx.Play(Sound.DeathEnemy, SoundPitch);
            Died?.Invoke();
            if (destroyOnDeath)
            {
                if (Random.value < heartDropChance) HeartPickup.Spawn(transform.position);
                Destroy(gameObject);
            }
            return;
        }
        Sfx.Play(Sound.HitEnemy, SoundPitch);
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        if (sr == null) yield break;
        sr.enabled = false;
        yield return new WaitForSeconds(blinkTime);
        sr.enabled = true;
    }
}
