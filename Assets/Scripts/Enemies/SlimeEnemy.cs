using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyVision))]
public class SlimeEnemy : MonoBehaviour
{
    enum State { Patrol, Chase, WindUp, Lunge, Cooldown }

    [Header("Patrol (2D waypoints)")]
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float patrolSpeed = 1.5f;

    [Header("Chase")]
    [SerializeField] float chaseSpeed = 3f;
    [SerializeField] float attackRange = 1.8f;
    [SerializeField] float loseSightTime = 1.5f;

    [Header("Attack")]
    [SerializeField] float windUpTime = 0.5f;
    [SerializeField] float lungeSpeed = 7f;
    [SerializeField] float lungeTime = 0.25f;
    [SerializeField] float cooldownTime = 1.2f;
    [SerializeField] int damage = 1;
    [SerializeField] float hitRadius = 0.8f;
    [SerializeField] Color windUpColor = new Color(0.89f, 0.29f, 0.29f);

    [Header("Debug")]
    [SerializeField] TMP_Text stateLabel;

    State state;
    float stateTimer;
    float lastSeenTime = -999f;
    int facing = 1;
    bool hitThisAttack;
    Transform patrolTarget;
    float minX, maxX;

    Rigidbody2D rb;
    SpriteRenderer sr;
    EnemyVision vision;
    PlayerHealth player;
    Color baseColor;
    Vector3 baseScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        vision = GetComponent<EnemyVision>();
        player = FindFirstObjectByType<PlayerHealth>();
        baseColor = sr.color;
        baseScale = transform.localScale;

        minX = Mathf.Min(pointA.position.x, pointB.position.x);
        maxX = Mathf.Max(pointA.position.x, pointB.position.x);
        patrolTarget = pointB;
        ChangeState(State.Patrol);
    }

    void Update()
    {
        stateTimer += Time.deltaTime;
        bool playerAlive = player != null && !player.IsDead;
        bool sees = playerAlive && vision.CanSee(facing);
        if (sees) lastSeenTime = Time.time;

        switch (state)
        {
            case State.Patrol:
                MoveTowards(patrolTarget.position.x, patrolSpeed);
                if (Mathf.Abs(transform.position.x - patrolTarget.position.x) < 0.1f)
                    patrolTarget = patrolTarget == pointA ? pointB : pointA;
                if (sees) ChangeState(State.Chase);
                break;

            case State.Chase:
                if (!playerAlive) { ChangeState(State.Patrol); break; }
                MoveTowards(player.transform.position.x, chaseSpeed);
                float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
                if (sees && distance < attackRange && EnemyUtils.IsOnScreen(transform.position))
                    ChangeState(State.WindUp);
                else if (Time.time - lastSeenTime > loseSightTime)
                    ChangeState(State.Patrol);
                break;

            case State.WindUp:
                SetVelocityX(0f);
                float t = stateTimer / windUpTime;
                transform.localScale = new Vector3(baseScale.x * (1f + 0.2f * t), baseScale.y * (1f - 0.3f * t), 1f);
                if (stateTimer >= windUpTime) ChangeState(State.Lunge);
                break;

            case State.Lunge:
                SetVelocityX(facing * lungeSpeed);
                TryHitPlayer();
                if (stateTimer >= lungeTime) ChangeState(State.Cooldown);
                break;

            case State.Cooldown:
                SetVelocityX(0f);
                if (stateTimer >= cooldownTime)
                    ChangeState(sees ? State.Chase : State.Patrol);
                break;
        }
    }

    void ChangeState(State next)
    {
        state = next;
        stateTimer = 0f;
        hitThisAttack = false;
        sr.color = next == State.WindUp ? windUpColor : baseColor;
        transform.localScale = baseScale;
        if (stateLabel != null) stateLabel.text = next.ToString();
    }

    void MoveTowards(float targetX, float speed)
    {
        float dx = targetX - transform.position.x;
        if (Mathf.Abs(dx) < 0.05f) { SetVelocityX(0f); return; }
        Face(dx > 0f ? 1 : -1);
        SetVelocityX(facing * speed);
    }

    void SetVelocityX(float vx)
    {
        float x = transform.position.x;
        if ((x <= minX && vx < 0f) || (x >= maxX && vx > 0f)) vx = 0f;
        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
    }

    void Face(int dir)
    {
        facing = dir;
        sr.flipX = dir < 0;
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
}
