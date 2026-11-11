using System.Collections;
using UnityEngine;
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 2;
    [SerializeField] int scoreValue = 100;
    [SerializeField] float blinkTime = 0.15f;

    public bool IsDead { get; private set; }

    int health;
    SpriteRenderer sr;

    void Awake()
    {
        health = maxHealth;
        sr = GetComponent<SpriteRenderer>();
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
