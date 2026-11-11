using TMPro;
using UnityEngine;

public class BatEnemy : MonoBehaviour
{
    enum State { Hover, WindUp, Dive, Return }

    [Header("Hover")]
    [SerializeField] float hoverAmplitude = 0.3f;
    [SerializeField] float hoverSpeed = 2f;
    [SerializeField] float detectRange = 5.5f;

    [Header("Attack")]
    [SerializeField] float windUpTime = 0.6f;
    [SerializeField] float shakeAmount = 0.08f;
    [SerializeField] float diveSpeed = 9f;
    [SerializeField] float maxDiveTime = 1.2f;
    [SerializeField] float returnSpeed = 4f;
    [SerializeField] float cooldownTime = 1.5f;
    [SerializeField] int damage = 1;
    [SerializeField] float hitRadius = 0.6f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Color windUpColor = new Color(0.89f, 0.29f, 0.29f);

    [Header("Debug")]
    [SerializeField] TMP_Text stateLabel;

    State state;
    float stateTimer;
    float nextAttackTime;
    bool hitThisAttack;
    Vector3 home;
    Vector3 windUpStart;
    Vector3 diveTarget;

    SpriteRenderer sr;
    PlayerHealth player;
    Color baseColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = FindFirstObjectByType<PlayerHealth>();
        baseColor = sr.color;
        home = transform.position;
        ChangeState(State.Hover);
    }

    void Update()
    {
        stateTimer += Time.deltaTime;
        bool playerAlive = player != null && !player.IsDead;

        switch (state)
        {
            case State.Hover:
                transform.position = home + Vector3.up * Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
                if (playerAlive && Time.time >= nextAttackTime
                    && Vector2.Distance(transform.position, player.transform.position) < detectRange
                    && EnemyUtils.IsOnScreen(transform.position))
                {
                    ChangeState(State.WindUp);
                }
                break;

            case State.WindUp:
                transform.position = windUpStart + (Vector3)(Random.insideUnitCircle * shakeAmount);
                sr.flipX = player != null && player.transform.position.x < transform.position.x;
                if (stateTimer >= windUpTime)
                {
                    transform.position = windUpStart;
                    ChangeState(State.Dive);
                }
                break;

            case State.Dive:
                transform.position = Vector3.MoveTowards(transform.position, diveTarget, diveSpeed * Time.deltaTime);
                TryHitPlayer();
                bool reached = Vector3.Distance(transform.position, diveTarget) < 0.05f;
                bool hitGround = Physics2D.OverlapCircle(transform.position, 0.2f, groundMask) != null;
                if (reached || hitGround || hitThisAttack || stateTimer >= maxDiveTime)
                    ChangeState(State.Return);
                break;

            case State.Return:
                transform.position = Vector3.MoveTowards(transform.position, home, returnSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, home) < 0.05f)
                {
                    nextAttackTime = Time.time + cooldownTime;
                    ChangeState(State.Hover);
                }
                break;
        }
    }

    void ChangeState(State next)
    {
        state = next;
        stateTimer = 0f;
        sr.color = next == State.WindUp ? windUpColor : baseColor;

        if (next == State.WindUp)
        {
            windUpStart = transform.position;
            diveTarget = player.transform.position;
            hitThisAttack = false;
        }
        if (stateLabel != null) stateLabel.text = next.ToString();
    }

    void TryHitPlayer()
    {
        if (hitThisAttack || player == null) return;
        if (Vector2.Distance(transform.position, player.transform.position) < hitRadius)
        {
            hitThisAttack = true;
            player.TakeDamage(damage, transform.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(Application.isPlaying ? home : transform.position, detectRange);
    }
}
