using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float blinkTime = 0.15f;

    public bool IsDead { get; private set; }

    int health;
    int scoreValue;
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Setup(int maxHealth, int score)
    {
        health = maxHealth;
        scoreValue = score;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;
        health -= damage;
        if (health <= 0)
        {
            IsDead = true;
            if (GameSession.Instance != null) GameSession.Instance.AddScore(scoreValue);
            Destroy(gameObject);
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
