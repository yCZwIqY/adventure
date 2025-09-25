using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] public float moveSpeed = 3f;
    public float jumpForce = 7f;
    public float minSwipeDist = 50f;

    [Header("Components")] private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    [Header("Effect")]
    public GameObject effect; 
    public GameObject dashEffect; 

    private Vector2 touchStartPos;
    private float touchStartTime;
    private int moveDir = 0; // -1=Left, 1=Right, 0=None
    private int direction = 1; // 0=Left, 1=Right
    private bool isGrounded = true;
    private float maxTapTime = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // 떨림 방지
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleTouchInput();
    }

    void FixedUpdate()
    {
        if (!isGrounded) return;

        if (moveDir != 0)
        {
            // PlayMoveParticles();
            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
            direction = moveDir > 0 ? 1 : 0;
            
            effect.gameObject.transform.rotation = Quaternion.Euler(0,  direction == 1 ? 180 : 0, 0);
            dashEffect.SetActive(true);
            SetAnimState(1); // Dash
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            dashEffect.SetActive(false);
            SetAnimState(0); // Idle
        }
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
                    if (swipe.magnitude >= minSwipeDist)
                    {
                        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                            moveDir = swipe.x > 0 ? 1 : -1;
                        else if (swipe.y > 0 && isGrounded)
                            Jump();
                    }

                    break;

                case TouchPhase.Ended:
                    float touchDuration = Time.time - touchStartTime;
                    if (touchDuration < maxTapTime && swipe.magnitude < minSwipeDist)
                    {
                        Attack();
                    }
                    else
                    {
                        moveDir = 0;
                    }

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
                if (swipe.magnitude >= minSwipeDist)
                {
                    if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                        moveDir = swipe.x > 0 ? 1 : -1;
                    else if (swipe.y > 0 && isGrounded)
                        Jump();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 swipe = (Vector2)Input.mousePosition - touchStartPos;
                float touchDuration = Time.time - touchStartTime;

                if (touchDuration < maxTapTime && swipe.magnitude < minSwipeDist)
                {
                    Attack();
                }
                else
                {
                    moveDir = 0;
                }
            }
        }
    }

    private void Jump()
    {
        if (!isGrounded) return;

        animator.SetTrigger("Jump");
        animator.SetBool("isGrounded", false);

        DoJump();
    }

    private void DoJump()
    {
        float currentX = rb.linearVelocity.x;

        float jumpX = currentX;
        if (Mathf.Approximately(currentX, 0f))
        {
            float smallBoost = 5f;
            jumpX = (direction == 1 ? smallBoost : -smallBoost);
        }

        rb.linearVelocity = new Vector2(jumpX, jumpForce);

        isGrounded = false;
    }


    public void Attack()
    {
        Debug.Log("Attack");
        animator.SetTrigger("Attack");
    }

    private void SetAnimState(int state)
    {
        animator.SetInteger("Direction", direction);
        animator.SetInteger("State", state);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            animator.SetBool("isGrounded", true);
        }
    }
    
}