using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class BossEnemy : MonoBehaviour
{
    enum State { Sleep, Fly, WindUp, Shoot, Dead }

    [SerializeField] BossStats stats;
    [SerializeField] string bossName = "FLYING DEMON";
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Vector2 mouthOffset = new Vector2(1.2f, 0.2f);
    [SerializeField] Color windUpColor = new Color(1f, 0.45f, 0.45f);
    [SerializeField] Color enragedColor = new Color(1f, 0.75f, 0.6f);

    [Header("Animation")]
    [SerializeField] Sprite[] flyFrames;
    [SerializeField] Sprite[] attackFrames;
    [SerializeField] Sprite[] deathFrames;
    [SerializeField] float framesPerSecond = 10f;

    [Header("Debug")]
    [SerializeField] TMP_Text stateLabel;

    State state;
    float stateTimer;
    float nextShotTime;
    bool enraged;
    int moveDir = -1;
    int facing = -1;
    float leftX;
    float rightX;
    float activateX;
    float baseY;

    EnemyHealth health;
    SpriteRenderer sr;
    PlayerHealth player;
    BossHealthBar bar;
    Color baseColor;

    Sprite[] clip;
    int frame;
    float frameTimer;
    bool loopClip = true;

    float Multiplier => enraged ? stats.phase2SpeedMultiplier : 1f;

    public void Setup(float left, float right, float activationX, BossHealthBar healthBar)
    {
        leftX = left;
        rightX = right;
        activateX = activationX;
        bar = healthBar;
    }

    void Awake()
    {
        health = GetComponent<EnemyHealth>();
        health.Setup(stats.maxHealth, stats.scoreValue);
        health.Damaged += OnDamaged;
        health.Died += OnDied;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = FindFirstObjectByType<PlayerHealth>();
        baseColor = sr.color;
        baseY = transform.position.y;
        if (rightX <= leftX)
        {
            leftX = transform.position.x - 5f;
            rightX = transform.position.x + 5f;
        }
        PlayClip(flyFrames, true);
        ChangeState(State.Sleep);
    }

    void Update()
    {
        Animate();
        if (state == State.Dead) return;

        stateTimer += Time.deltaTime;
        bool playerAlive = player != null && !player.IsDead;
        if (playerAlive) Face(player.transform.position.x > transform.position.x ? 1 : -1);

        switch (state)
        {
            case State.Sleep:
                Hover(0f);
                if (playerAlive && player.transform.position.x >= activateX && EnemyUtils.IsOnScreen(transform.position))
                {
                    if (bar != null) bar.Show(bossName);
                    nextShotTime = Time.time + stats.shootInterval;
                    ChangeState(State.Fly);
                }
                break;

            case State.Fly:
                Hover(stats.flySpeed * Multiplier);
                if (playerAlive && Time.time >= nextShotTime && EnemyUtils.IsOnScreen(transform.position))
                    ChangeState(State.WindUp);
                break;

            case State.WindUp:
                Hover(0f);
                sr.color = Mathf.Repeat(stateTimer * 12f, 1f) < 0.5f ? windUpColor : CurrentBaseColor();
                if (stateTimer >= stats.windUpTime / Multiplier) ChangeState(State.Shoot);
                break;

            case State.Shoot:
                Fire();
                nextShotTime = Time.time + stats.shootInterval / Multiplier;
                ChangeState(State.Fly);
                break;
        }
    }

    void Hover(float speed)
    {
        Vector3 p = transform.position;
        if (speed > 0f)
        {
            p.x += moveDir * speed * Time.deltaTime;
            if (p.x <= leftX) { p.x = leftX; moveDir = 1; }
            if (p.x >= rightX) { p.x = rightX; moveDir = -1; }
        }
        p.y = baseY + Mathf.Sin(Time.time * stats.hoverSpeed) * stats.hoverAmplitude;
        transform.position = p;
    }

    void Fire()
    {
        if (projectilePrefab == null || player == null) return;

        Vector2 mouth = (Vector2)transform.position + new Vector2(mouthOffset.x * facing, mouthOffset.y);
        Vector2 aim = ((Vector2)player.transform.position - mouth).normalized;
        int count = enraged ? stats.phase2ProjectileCount : 1;

        for (int i = 0; i < count; i++)
        {
            float angle = (i - (count - 1) * 0.5f) * stats.phase2SpreadAngle;
            Vector2 dir = Quaternion.Euler(0f, 0f, angle) * aim;
            Projectile shot = Instantiate(projectilePrefab, mouth, Quaternion.identity);
            shot.Launch(dir, stats.projectileSpeed, stats.projectileDamage);
        }
    }

    void OnDamaged()
    {
        if (bar != null) bar.SetFraction(health.Fraction);
        if (!enraged && !health.IsDead && health.Fraction <= stats.phase2HealthFraction)
        {
            enraged = true;
            if (bar != null) bar.SetEnraged(bossName + "  (ENRAGED)");
            if (state != State.WindUp) sr.color = CurrentBaseColor();
            UpdateLabel();
        }
    }

    void OnDied()
    {
        state = State.Dead;
        sr.enabled = true;
        sr.color = Color.white;
        if (bar != null) bar.SetFraction(0f);
        if (stateLabel != null) stateLabel.text = "Dead";
        foreach (Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        PlayClip(deathFrames, false);
        float length = deathFrames != null && deathFrames.Length > 0 ? deathFrames.Length / framesPerSecond : 0.5f;
        yield return new WaitForSeconds(length + 0.5f);
        if (bar != null) bar.Hide();
        if (GameSession.Instance != null) GameSession.Instance.LevelComplete();
        gameObject.SetActive(false);
    }

    void ChangeState(State next)
    {
        state = next;
        stateTimer = 0f;
        sr.color = CurrentBaseColor();
        if (next == State.WindUp) PlayClip(attackFrames, false);
        else if (next == State.Fly || next == State.Sleep) PlayClip(flyFrames, true);
        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (stateLabel == null) return;
        stateLabel.text = enraged ? state + " x" + stats.phase2SpeedMultiplier : state.ToString();
    }

    Color CurrentBaseColor()
    {
        return enraged ? enragedColor : baseColor;
    }

    void Face(int dir)
    {
        facing = dir;
        sr.flipX = dir > 0;
    }

    void PlayClip(Sprite[] frames, bool loop)
    {
        if (frames == null || frames.Length == 0) return;
        clip = frames;
        loopClip = loop;
        frame = 0;
        frameTimer = 0f;
        sr.sprite = clip[0];
    }

    void Animate()
    {
        if (clip == null || clip.Length == 0) return;
        frameTimer += Time.deltaTime;
        if (frameTimer < 1f / framesPerSecond) return;
        frameTimer = 0f;
        if (frame + 1 < clip.Length) frame++;
        else if (loopClip) frame = 0;
        sr.sprite = clip[frame];
    }
}
