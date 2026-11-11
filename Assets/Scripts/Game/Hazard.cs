using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hazard : MonoBehaviour
{
    [SerializeField] Color colorA = new Color(0.94f, 0.42f, 0.12f);
    [SerializeField] Color colorB = new Color(0.98f, 0.78f, 0.25f);
    [SerializeField] float pulseSpeed = 3f;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (sr == null) return;
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        sr.color = Color.Lerp(colorA, colorB, t);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player != null) player.Kill();
    }
}
