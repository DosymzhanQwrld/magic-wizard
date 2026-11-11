using TMPro;
using UnityEngine;

[RequireComponent(typeof(EnemyVision))]
public class ArcherEnemy : MonoBehaviour
{
    enum State { Idle, Aim, Shoot, Cooldown }

    [Header("Idle")]
    [SerializeField] int startFacing = -1;
    [SerializeField] float turnInterval = 3f;

    [Header("Attack")]
    [SerializeField] float windUpTime = 0.6f;
    [SerializeField] float cooldownTime = 1.6f;
    [SerializeField] Projectile arrowPrefab;
    [SerializeField] Vector2 bowOffset = new Vector2(0.5f, 0.2f);
    [SerializeField] LineRenderer aimLine;
    [SerializeField] Color windUpColor = new Color(0.89f, 0.29f, 0.29f);

    [Header("Debug")]
    [SerializeField] TMP_Text stateLabel;

    State state;
    float stateTimer;
    int facing;
    Vector2 aimDirection;

    SpriteRenderer sr;
    EnemyVision vision;
    PlayerHealth player;
    Color baseColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        vision = GetComponent<EnemyVision>();
        player = FindFirstObjectByType<PlayerHealth>();
        baseColor = sr.color;
        Face(startFacing);
        ChangeState(State.Idle);
    }

    void Update()
    {
        stateTimer += Time.deltaTime;
        bool playerAlive = player != null && !player.IsDead;
        bool sees = playerAlive && vision.CanSee(facing);

        switch (state)
        {
            case State.Idle:
                if (stateTimer >= turnInterval) { Face(-facing); stateTimer = 0f; }
                if (sees && EnemyUtils.IsOnScreen(transform.position)) ChangeState(State.Aim);
                break;

            case State.Aim:
                if (!sees) { ChangeState(State.Idle); break; }
                Face(player.transform.position.x > transform.position.x ? 1 : -1);
                Vector2 bow = BowPosition();
                aimDirection = ((Vector2)player.transform.position - bow).normalized;
                aimLine.SetPosition(0, bow);
                aimLine.SetPosition(1, player.transform.position);
                float blink = Mathf.Lerp(6f, 20f, stateTimer / windUpTime);
                aimLine.enabled = Mathf.Sin(stateTimer * blink) > -0.3f;
                if (stateTimer >= windUpTime) ChangeState(State.Shoot);
                break;

            case State.Shoot:
                if (arrowPrefab != null)
                {
                    Projectile arrow = Instantiate(arrowPrefab, BowPosition(), Quaternion.identity);
                    arrow.Launch(aimDirection);
                }
                ChangeState(State.Cooldown);
                break;

            case State.Cooldown:
                if (stateTimer >= cooldownTime)
                    ChangeState(sees ? State.Aim : State.Idle);
                break;
        }
    }

    void ChangeState(State next)
    {
        state = next;
        stateTimer = 0f;
        sr.color = next == State.Aim ? windUpColor : baseColor;
        if (aimLine != null) aimLine.enabled = next == State.Aim;
        if (stateLabel != null) stateLabel.text = next.ToString();
    }

    Vector2 BowPosition()
    {
        return (Vector2)transform.position + new Vector2(bowOffset.x * facing, bowOffset.y);
    }

    void Face(int dir)
    {
        facing = dir;
        sr.flipX = dir < 0;
    }
}
