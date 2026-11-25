using UnityEngine;

// A heart that a killed enemy can drop. Touching it heals 1 heart and plays the pickup sound.
public class HeartPickup : MonoBehaviour
{
    const float Size = 0.6f;

    [SerializeField] int heal = 1;
    [SerializeField] float bobHeight = 0.12f;
    [SerializeField] float bobSpeed = 3f;
    [SerializeField] float lifeTime = 12f;

    static Sprite sprite;
    Vector3 start;

    public static void Spawn(Vector3 position)
    {
        if (sprite == null) sprite = Resources.Load<Sprite>("Heart");

        GameObject go = new GameObject("Heart Pickup");
        go.transform.position = position + Vector3.up * 0.3f;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 10;
        if (sprite != null) go.transform.localScale = Vector3.one * (Size / sprite.bounds.size.x);

        CircleCollider2D trigger = go.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;

        go.AddComponent<HeartPickup>();
    }

    void Start()
    {
        start = transform.position;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position = start + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player == null || player.IsDead) return;
        player.Heal(heal);
        Sfx.Play(Sound.Pickup);
        Destroy(gameObject);
    }
}
