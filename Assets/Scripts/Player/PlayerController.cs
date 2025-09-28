using Drakkar.GameUtils;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [FormerlySerializedAs("moveSpeed")] [Header("Movement Settings")]
    public float runSpeed = 55f;

    public float walkSpeed = 30f;
    public float dashAccelerationTime = 1f;
    public float dashForce = 10f;
    private bool isDashing = false;
    private float dashVelocityXSmoothing = 0f;

    public float jumpForce = 7f;
    public float minSwipeDist = 50f;

    public float defendHoldTime = 0.5f;

    [Header("Components")] private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Effect")] public GameObject effect;
    public GameObject dashEffect;
    private Vector2 dashEffectBasePos;
    public GameObject attackEffect;
    public GameObject groundHitEffect;

    [Header("Health")] public int maxHealth = 5;
    public int health = 5;

    private Vector2 touchStartPos;
    private float touchStartTime;
    private int moveDir = 0; // -1=Left, 1=Right, 0=None
    private int direction = 1; // 0=Left, 1=Right
    private bool isGrounded = true;
    private float maxTapTime = 0.2f;

    [Header("Animation Speed Control")] [Range(0.05f, 1f)]
    public float animationSpeedMultiplier = 0.1f;

    public float minAnimSpeed = 0.5f;
    public float maxAnimSpeed = 2.0f;

    [Header("Combat")] // TODO: 콤보공격
    public float comboResetTime = 0.6f;

    private int comboStep = 0;
    private float lastAttackTime = 0f;

    public float power = 1;
    public bool isAttack = false;
    public bool isDefending = false;
    public CircleCollider2D attackRange;

    private static class AnimParams
    {
        public const string IsGrounded = "IsGrounded";
        public const string Attack = "Attack";
        public const string Jump = "Jump";
        public const string JumpDash = "JumpDash";
        public const string Direction = "Direction";
        public const string State = "State";
        public const string MoveSpeed = "MoveSpeed";
        public const string IsDefense = "IsDefense";
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackRange = GetComponent<CircleCollider2D>();

        DrakkarTrail dashTrail = dashEffect.GetComponent<DrakkarTrail>();
        dashTrail.Begin();
        dashEffectBasePos = dashEffect.transform.localPosition;

        UIManager.Instance.RenderPlayerHealth(health);
    }

    void Update()
    {
        HandleTouchInput();
        UpdateAnimatorSpeed();
        UpdateAnimationState();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float targetSpeed = moveDir != 0
            ? (animator.GetFloat(AnimParams.MoveSpeed) >= 3f ? runSpeed : walkSpeed) * moveDir
            : 0f;
        float smoothTime = moveDir != 0 ? dashAccelerationTime : dashAccelerationTime * 0.5f;

        if (isGrounded)
        {
            float newX = Mathf.SmoothDamp(rb.linearVelocity.x, targetSpeed, ref dashVelocityXSmoothing, smoothTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }

        if (!isGrounded && !isDashing && moveDir != 0 && Mathf.Abs(rb.linearVelocity.x) < 0.3f)
        {
            rb.AddForce(new Vector2(moveDir * dashForce, 0), ForceMode2D.Impulse);
            animator.SetTrigger(AnimParams.JumpDash);
            dashEffect.SetActive(true);
            isDashing = true;
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 swipe = touch.position - touchStartPos;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    touchStartTime = Time.time;
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    ProcessSwipe(swipe, false);
                    break;
                case TouchPhase.Ended:
                    ProcessSwipe(swipe, true, Time.time - touchStartTime);
                    break;
            }
        }

        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
                touchStartTime = Time.time;
            }
            else if (Input.GetMouseButton(0))
            {
                ProcessSwipe((Vector2)Input.mousePosition - touchStartPos, false);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ProcessSwipe((Vector2)Input.mousePosition - touchStartPos, true, Time.time - touchStartTime);
            }
        }
    }

    private void ProcessSwipe(Vector2 swipe, bool isRelease, float duration = 0f)
    {
        if (!isRelease)
        {
            float holdTime = Time.time - touchStartTime;

            // 길게 누르기 방어 체크 (한 번만 호출)
            if (holdTime > defendHoldTime && !isDefending && swipe.magnitude < minSwipeDist)
            {
                StartDefend();
                return; // 방어 시작하면 더 이상 이동 처리 안 함
            }
            
            if (swipe.magnitude >= minSwipeDist)
            {
                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                {
                    moveDir = swipe.x > 0 ? 1 : -1;
                    float swipeRatio = swipe.magnitude / Screen.width;

                    if (swipeRatio <= 0.05f)
                    {
                        animator.SetFloat(AnimParams.MoveSpeed, 0f);
                        dashEffect.SetActive(false);
                    }
                    else if (swipeRatio <= 0.6f)
                    {
                        animator.SetFloat(AnimParams.MoveSpeed, 1.5f);
                        dashEffect.SetActive(false);
                    }
                    else
                    {
                        animator.SetFloat(AnimParams.MoveSpeed, 3f);
                        dashEffect.SetActive(true);
                        isDashing = true;
                    }
                }
                else if (swipe.y > 0 && isGrounded)
                {
                    animator.SetFloat(AnimParams.MoveSpeed, 0f);
                    dashEffect.SetActive(false);
                    Jump();
                }
            }
        }
        else
        {
            animator.SetFloat(AnimParams.MoveSpeed, 0f);
            if (isDefending)
            {
                isDefending = false;
                animator.SetBool(AnimParams.IsDefense, false);
                return;
            }

            if (duration < maxTapTime && swipe.magnitude < minSwipeDist)
                Attack();
            else
                moveDir = 0;
        }
    }

    public void StartDefend()
    {
        if (isDefending) return; // 이미 방어 중이면 무시

        isDefending = true;
        animator.SetBool(AnimParams.IsDefense, true); // 한 번만 true 세팅
        moveDir = 0; // 이동 즉시 정지
        rb.linearVelocity = Vector2.zero;
    }

    public void EndDefend()
    {
        if (!isDefending) return;

        isDefending = false;
        animator.SetBool(AnimParams.IsDefense, false);
    }
    private void Jump()
    {
        if (!isGrounded) return;

        dashEffect.transform.localPosition = new Vector2(0, dashEffectBasePos.y);
        animator.SetTrigger(AnimParams.Jump);
        animator.SetBool(AnimParams.IsGrounded, false);
        Invoke(nameof(DoJump), 0.2f);
    }

    private void DoJump()
    {
        float currentX = rb.linearVelocity.x;
        float jumpX = Mathf.Abs(currentX) > runSpeed * 0.8f
            ? currentX
            : (Mathf.Approximately(currentX, 0f) ? (direction == 1 ? 5f : -5f) : currentX * 0.3f);
        rb.linearVelocity = new Vector2(jumpX, jumpForce);
        isGrounded = false;
    }

    public void Attack()
    {
        if (!isGrounded) return;

        animator.SetTrigger(AnimParams.Attack);
        attackRange.enabled = true;
        isAttack = true;
        SpawnAttackEffect();
    }

    public void SpawnAttackEffect()
    {
        Vector3 spawnPos = effect.transform.position;
        Quaternion spawnRot = Quaternion.Euler(0f, moveDir > 0 ? 0f : 180f, 0f);
        Instantiate(attackEffect, spawnPos, spawnRot, transform);
    }

    public void AttackEnd()
    {
        attackRange.enabled = false;
        isAttack = false;
    }

    private void UpdateAnimationState()
    {
        direction = moveDir != 0 ? (moveDir > 0 ? 1 : 0) : direction;

        effect.transform.rotation = Quaternion.Euler(0, direction == 1 ? 180 : 0, 0);
        dashEffect.transform.localPosition = new Vector2(0.6f * (moveDir > 0 ? 1 : -1), dashEffectBasePos.y);
        SetAnimState(moveDir != 0 ? 1 : 0);
    }

    private void SetAnimState(int state)
    {
        animator.SetInteger(AnimParams.Direction, direction);
        animator.SetInteger(AnimParams.State, state);
    }

    private void UpdateAnimatorSpeed()
    {
        float targetSpeed = Mathf.Clamp(Mathf.Abs(rb.linearVelocity.x) * animationSpeedMultiplier, minAnimSpeed,
            maxAnimSpeed);
        animator.speed = Mathf.Lerp(animator.speed, targetSpeed, Time.deltaTime * 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            animator.SetBool(AnimParams.IsGrounded, true);
            GameObject eff = Instantiate(groundHitEffect, transform);
            eff.transform.localPosition = new Vector3(-0.15f, -0.81f, 0f);
            eff.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            isDashing = false;
        }
    }

    public void OnHit(int damage)
    {
        health = Mathf.Max(0, health - damage);
        UIManager.Instance.RenderPlayerHealth(health);
        if (health <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("Player Dead");
    }
}