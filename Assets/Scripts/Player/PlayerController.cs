using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float jumpVelocity = 12f;
    [SerializeField] float stickDeadZone = 0.2f;

    [Header("Ground check")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.7f, 0.1f);
    [SerializeField] float groundCheckOffset = 0.5f;

    [Header("Mobile controls")]
    [SerializeField] MobileJoystick joystick;
    [SerializeField] MobileButton jumpButton;

    public int Facing { get; private set; } = 1;

    public bool InputLocked { get; set; }

    Rigidbody2D rb;
    SpriteRenderer sr;
    InputAction moveAction;
    InputAction jumpAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Axis");
        moveAction.AddBinding("<Gamepad>/leftStick/x");
        moveAction.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/a").With("Positive", "<Keyboard>/d");
        moveAction.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/leftArrow").With("Positive", "<Keyboard>/rightArrow");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Keyboard>/w");
        jumpAction.AddBinding("<Keyboard>/upArrow");
    }

    void OnEnable()  { moveAction.Enable();  jumpAction.Enable(); }
    void OnDisable() { moveAction.Disable(); jumpAction.Disable(); }

    void Update()
    {
        if (Time.timeScale == 0f || InputLocked) return;

        float x = moveAction.ReadValue<float>();
        if (joystick != null && Mathf.Abs(joystick.Value.x) > Mathf.Abs(x)) x = joystick.Value.x;
        if (Mathf.Abs(x) < stickDeadZone) x = 0f;

        bool jumpPressed = jumpAction.WasPressedThisFrame();
        if (jumpButton != null && jumpButton.ConsumePress()) jumpPressed = true;

        if (x != 0f)
        {
            Facing = x > 0f ? 1 : -1;
            if (sr) sr.flipX = Facing < 0;
        }

        Vector2 v = rb.linearVelocity;
        v.x = x * moveSpeed;
        if (jumpPressed && IsGrounded())
            v.y = jumpVelocity;
        rb.linearVelocity = v;
    }

    public bool IsGrounded()
    {
        Vector2 feet = (Vector2)transform.position + Vector2.down * groundCheckOffset;
        return Physics2D.OverlapBox(feet, groundCheckSize, 0f, groundMask) != null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
    }
}
