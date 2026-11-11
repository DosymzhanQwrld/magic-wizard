using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerShooter : MonoBehaviour
{
    [SerializeField] Projectile fireballPrefab;
    [SerializeField] float cooldown = 0.5f;
    [SerializeField] Vector2 spawnOffset = new Vector2(0.6f, 0.1f);
    [SerializeField] MobileButton fireButton;

    PlayerController controller;
    PlayerHealth health;
    InputAction fireAction;
    float nextShotTime;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        health = GetComponent<PlayerHealth>();

        fireAction = new InputAction("Fire", InputActionType.Button);
        fireAction.AddBinding("<Gamepad>/buttonWest");
        fireAction.AddBinding("<Keyboard>/j");
        fireAction.AddBinding("<Keyboard>/k");
    }

    void OnEnable()  { fireAction.Enable(); }
    void OnDisable() { fireAction.Disable(); }

    void Update()
    {
        if (Time.timeScale == 0f || fireballPrefab == null) return;
        if (health != null && health.IsDead) return;

        bool firePressed = fireAction.WasPressedThisFrame();
        if (fireButton != null && fireButton.ConsumePress()) firePressed = true;

        if (firePressed && Time.time >= nextShotTime)
        {
            nextShotTime = Time.time + cooldown;
            int dir = controller.Facing;
            Vector2 pos = (Vector2)transform.position + new Vector2(spawnOffset.x * dir, spawnOffset.y);
            Projectile fireball = Instantiate(fireballPrefab, pos, Quaternion.identity);
            fireball.Launch(new Vector2(dir, 0f));
        }
    }
}
