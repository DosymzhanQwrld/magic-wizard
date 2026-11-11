using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] int damage = 1;
    [SerializeField] float lifeTime = 3f;
    [SerializeField] bool hitsPlayer;
    [SerializeField] LayerMask groundMask;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    public void Launch(Vector2 direction)
    {
        Vector2 dir = direction.normalized;
        rb.linearVelocity = dir * speed;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((groundMask.value & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(gameObject);
            return;
        }

        if (hitsPlayer)
        {
            PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage, transform.position);
                Destroy(gameObject);
            }
        }
        else
        {
            EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
