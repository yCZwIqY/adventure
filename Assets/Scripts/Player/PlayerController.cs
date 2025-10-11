using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerInputHandler input;
    public PlayerMovement movement;
    public PlayerCombat combat;
    public PlayerAnimationHandler animHandler;
    public PlayerHealth health;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 자동 연결
        input = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        animHandler = GetComponent<PlayerAnimationHandler>();
        health = GetComponent<PlayerHealth>();

        // 초기화
        movement.Initialize(this);
        combat.Initialize(this);
        animHandler.Initialize(this);
        health.Initialize(this);
    }

    private void Update()
    {
        input.HandleInput();
        animHandler.UpdateAnimatorSpeed();
    }

    private void FixedUpdate()
    {
        movement.HandleMovement();
    }
}