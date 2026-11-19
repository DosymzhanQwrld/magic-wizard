using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float blinkTime = 0.15f;
    [SerializeField] bool destroyOnDeath = true;

    public bool IsDead { get; private set; }
    public int Max { get; private set; }
    public float Fraction => Max > 0 ? (float)health / Max : 0f;

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
            Died?.Invoke();
            if (destroyOnDeath) Destroy(gameObject);
            return;
        }
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
