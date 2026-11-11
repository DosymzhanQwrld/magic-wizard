using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHearts = 3;
    [SerializeField] float invincibleTime = 1f;
    [SerializeField] Vector2 knockback = new Vector2(5f, 6f);
    [SerializeField] float knockbackLockTime = 0.25f;
    [SerializeField] float fallDeathY = -6f;
    [SerializeField] Image[] heartImages;

    public bool IsDead { get; private set; }

    int hearts;
    float invincibleUntil;
    Rigidbody2D rb;
    SpriteRenderer sr;
    PlayerController controller;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        controller = GetComponent<PlayerController>();
        hearts = maxHearts;
        UpdateHearts();
    }

    void Update()
    {
        if (!IsDead && transform.position.y < fallDeathY)
            Die();
    }

    public void TakeDamage(int damage, Vector2 fromPosition)
    {
        if (IsDead || Time.time < invincibleUntil) return;

        hearts = Mathf.Max(0, hearts - damage);
        UpdateHearts();
        if (hearts == 0) { Die(); return; }

        invincibleUntil = Time.time + invincibleTime;
        float dir = Mathf.Sign(transform.position.x - fromPosition.x);
        if (dir == 0f) dir = 1f;
        rb.linearVelocity = new Vector2(dir * knockback.x, knockback.y);
        StartCoroutine(HurtRoutine());
    }

    IEnumerator HurtRoutine()
    {
        controller.InputLocked = true;
        yield return new WaitForSeconds(knockbackLockTime);
        controller.InputLocked = false;

        while (Time.time < invincibleUntil)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.08f);
        }
        sr.enabled = true;
    }

    public void Kill()
    {
        Die();
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;
        hearts = 0;
        UpdateHearts();
        controller.enabled = false;
        rb.linearVelocity = Vector2.zero;
        sr.enabled = true;
        sr.color = Color.gray;
        if (GameSession.Instance != null) GameSession.Instance.PlayerDied();
    }

    void UpdateHearts()
    {
        if (heartImages == null) return;
        for (int i = 0; i < heartImages.Length; i++)
            if (heartImages[i] != null) heartImages[i].enabled = i < hearts;
    }
}
