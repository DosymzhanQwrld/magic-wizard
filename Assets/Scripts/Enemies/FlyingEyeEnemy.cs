using TMPro;
using UnityEngine;

public class FlyingEyeEnemy : MonoBehaviour
{
    enum State { Hover, WindUp, Dive, Return }

    [Header("Attack")]
    [SerializeField] FlyingEyeStats stats;
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

    void Awake()
    {
        GetComponent<EnemyHealth>().Setup(stats.maxHealth, stats.scoreValue);
    }

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
                transform.position = home + Vector3.up * Mathf.Sin(Time.time * stats.hoverSpeed) * stats.hoverAmplitude;
                if (playerAlive && Time.time >= nextAttackTime
                    && Vector2.Distance(transform.position, player.transform.position) < stats.detectRange
                    && EnemyUtils.IsOnScreen(transform.position))
                {
                    ChangeState(State.WindUp);
                }
                break;

            case State.WindUp:
                transform.position = windUpStart + (Vector3)(Random.insideUnitCircle * stats.shakeAmount);
                sr.flipX = player != null && player.transform.position.x < transform.position.x;
                if (stateTimer >= stats.windUpTime)
                {
                    transform.position = windUpStart;
                    ChangeState(State.Dive);
                }
                break;

            case State.Dive:
                transform.position = Vector3.MoveTowards(transform.position, diveTarget, stats.diveSpeed * Time.deltaTime);
                TryHitPlayer();
                bool reached = Vector3.Distance(transform.position, diveTarget) < 0.05f;
                bool hitGround = Physics2D.OverlapCircle(transform.position, 0.2f, groundMask) != null;
                if (reached || hitGround || hitThisAttack || stateTimer >= stats.maxDiveTime)
                    ChangeState(State.Return);
                break;

            case State.Return:
                transform.position = Vector3.MoveTowards(transform.position, home, stats.returnSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, home) < 0.05f)
                {
                    nextAttackTime = Time.time + stats.cooldownTime;
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
            Sfx.Play(Sound.WindupEye);
        }
        if (stateLabel != null) stateLabel.text = next.ToString();
    }

    void TryHitPlayer()
    {
        if (hitThisAttack || player == null) return;
        if (Vector2.Distance(transform.position, player.transform.position) < stats.hitRadius)
        {
            hitThisAttack = true;
            player.TakeDamage(stats.damage, transform.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        if (stats != null) Gizmos.DrawWireSphere(Application.isPlaying ? home : transform.position, stats.detectRange);
    }
}
