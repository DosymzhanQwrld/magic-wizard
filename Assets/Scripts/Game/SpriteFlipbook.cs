using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlipbook : MonoBehaviour
{
    [SerializeField] Sprite[] idleFrames;
    [SerializeField] Sprite[] moveFrames;
    [SerializeField] Sprite[] airFrames;
    [SerializeField] float framesPerSecond = 10f;
    [SerializeField] float moveThreshold = 0.3f;

    SpriteRenderer sr;
    PlayerController controller;
    Sprite[] current;
    float timer;
    int index;
    float lastX;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        controller = GetComponent<PlayerController>();
        lastX = transform.position.x;
    }

    void Update()
    {
        if (Time.deltaTime <= 0f) return;

        float speed = Mathf.Abs(transform.position.x - lastX) / Time.deltaTime;
        lastX = transform.position.x;

        Sprite[] next = idleFrames;
        if (controller != null && airFrames != null && airFrames.Length > 0 && !controller.IsGrounded()) next = airFrames;
        else if (moveFrames != null && moveFrames.Length > 0 && speed > moveThreshold) next = moveFrames;

        if (next == null || next.Length == 0) return;

        if (next != current)
        {
            current = next;
            index = 0;
            timer = 0f;
            sr.sprite = current[0];
            return;
        }

        timer += Time.deltaTime;
        if (timer >= 1f / framesPerSecond)
        {
            timer = 0f;
            index = (index + 1) % current.Length;
            sr.sprite = current[index];
        }
    }
}
