using Drakkar.GameUtils;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] 
    public float moveSpeed = 3f;
    public float dashAccelerationTime = 1f; // 목표 속도까지 가속하는 시간
    public float dashForce = 10f; 
    private bool isDashing = false;
    
    private float dashVelocityXSmoothing = 0f;

    public float jumpForce = 7f;
    public float minSwipeDist = 50f;

    [Header("Components")] 
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Effect")] 
    public GameObject effect;
    public GameObject dashEffect;
    private Vector2 dashEffectBasePos;
    public GameObject attackEffect;
    public GameObject groundHitEffect;

    [Header("Health")] 
    public int maxHealth = 5; 
    public int health = 5; 

    private Vector2 touchStartPos;
    private float touchStartTime;
    private int moveDir = 0; // -1=Left, 1=Right, 0=None
    private int direction = 1; // 0=Left, 1=Right
    private bool isGrounded = true;
    private float maxTapTime = 0.2f;

    [Header("Animation Speed Control")]
    [Range(0.05f, 1f)] public float animationSpeedMultiplier = 0.1f;
    public float minAnimSpeed = 0.5f;
    public float maxAnimSpeed = 2.0f;

    // Animator 파라미터 문자열을 상수로 관리
    private static class AnimParams
    {
        public const string IsGrounded = "isGrounded";
        public const string Attack = "Attack";
        public const string Jump = "Jump";
        public const string JumpDash = "JumpDash";
        public const string Direction = "Direction";
        public const string State = "State";
        public const string Speed = "speed";
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // 떨림 방지
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        DrakkarTrail dashTrail = dashEffect.GetComponent<DrakkarTrail>();
        dashTrail.Begin();
        dashEffectBasePos = dashEffect.transform.localPosition;

        UIManager.Instance.RenderPlayerHealth(health);
    }

    void Update()
    {
        HandleTouchInput();
        UpdateAnimatorSpeed();
        UpdateAnimationState(); // 애니메이션 상태는 Update에서 처리
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float target = moveDir != 0 ? moveDir * moveSpeed : 0f;
        float smoothTime = moveDir != 0 ? dashAccelerationTime : dashAccelerationTime * 0.5f;

        if (isGrounded)
        {
            // 지상에서만 감속/가속
            float newX = Mathf.SmoothDamp(rb.linearVelocity.x, target, ref dashVelocityXSmoothing, smoothTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }

        // 점프 중 대쉬
        if (CanAirDash())
        {
            rb.AddForce(new Vector2(moveDir * dashForce, 0), ForceMode2D.Impulse);
            animator.SetTrigger(AnimParams.JumpDash);
            isDashing = true;
        }
    }

    private bool CanAirDash()
    {
        return !isGrounded && !isDashing && moveDir != 0 && Mathf.Abs(rb.linearVelocity.x) < 0.3f;
    }

    private void HandleTouchInput()
    {
        // 모바일 터치
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
                    float touchDuration = Time.time - touchStartTime;
                    ProcessSwipe(swipe, true, touchDuration);
                    break;
            }
        }

        // 에디터 마우스 테스트
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
                touchStartTime = Time.time;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 swipe = (Vector2)Input.mousePosition - touchStartPos;
                ProcessSwipe(swipe, false);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 swipe = (Vector2)Input.mousePosition - touchStartPos;
                float touchDuration = Time.time - touchStartTime;
                ProcessSwipe(swipe, true, touchDuration);
            }
        }
    }

    private void ProcessSwipe(Vector2 swipe, bool isRelease, float duration = 0f)
    {
        if (!isRelease)
        {
            if (swipe.magnitude >= minSwipeDist)
            {
                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                    moveDir = swipe.x > 0 ? 1 : -1;
                else if (swipe.y > 0 && isGrounded)
                    Jump();
            }
        }
        else
        {
            if (duration < maxTapTime && swipe.magnitude < minSwipeDist)
                Attack();
            else
                moveDir = 0;
        }
    }

    private void Jump()
    {
        if (!isGrounded) return;

        dashEffect.transform.localPosition = new Vector2(0, dashEffectBasePos.y);

        animator.SetTrigger(AnimParams.Jump);
        animator.SetBool(AnimParams.IsGrounded, false);

        DoJump();
    }

    private void DoJump()
    {
        float currentX = rb.linearVelocity.x;
        float jumpX;

        if (Mathf.Abs(currentX) > moveSpeed * 0.8f)
        {
            jumpX = currentX;
        }
        else
        {
            float smallBoost = 5f;
            if (Mathf.Approximately(currentX, 0f))
                jumpX = (direction == 1 ? smallBoost : -smallBoost);
            else
                jumpX = currentX * 0.3f;
        }

        rb.linearVelocity = new Vector2(jumpX, jumpForce);
        isGrounded = false;
    }

    public void Attack()
    {
        animator.SetTrigger(AnimParams.Attack);
        Vector3 spawnPos = effect.transform.position;
        Quaternion spawnRot = Quaternion.Euler(0f, moveDir > 0 ? 0f : 180f, 0f);
        Instantiate(attackEffect, spawnPos, spawnRot, transform);
    }

    private void UpdateAnimationState()
    {
        if (moveDir != 0)
        {
            direction = moveDir > 0 ? 1 : 0;
            effect.transform.rotation = Quaternion.Euler(0, direction == 1 ? 180 : 0, 0);
            dashEffect.transform.localPosition = new Vector2(
                1.5f * (moveDir > 0 ? 1 : -1),
                dashEffectBasePos.y
            );
            SetAnimState(1); // Dash
        }
        else
        {
            SetAnimState(0); // Idle
        }
    }

    private void SetAnimState(int state)
    {
        animator.SetInteger(AnimParams.Direction, direction);
        animator.SetInteger(AnimParams.State, state);
    }

    private void UpdateAnimatorSpeed()
    {
        float moveSpeedValue = rb.linearVelocity.magnitude;
        float targetAnimSpeed = Mathf.Clamp(moveSpeedValue * animationSpeedMultiplier, minAnimSpeed, maxAnimSpeed);
        animator.speed = Mathf.Lerp(animator.speed, targetAnimSpeed, Time.deltaTime * 5f);

        animator.SetFloat(AnimParams.Speed, moveSpeedValue);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            animator.SetBool(AnimParams.IsGrounded, true);
            GameObject effect = Instantiate(groundHitEffect, transform); 
            effect.transform.localPosition = new Vector3(-0.1f, -1.7f, 0f); 
            effect.transform.localRotation = Quaternion.Euler(-90, 0f, 0f);
            isDashing = false;
        }
    }
    
    public void OnHit(int damage)
    {
        health = Mathf.Max(0, health - damage);
        UIManager.Instance.RenderPlayerHealth(health);

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player Dead");
        // TODO: 사망 연출/리트라이 처리
    }
}
