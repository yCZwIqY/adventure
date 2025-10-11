using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;
    private Rigidbody2D rb;

    public GameObject runEffect;
    public GameObject groundHitEffect;
    public GameObject dashEffect;

    public float walkSpeed = 30f;
    public float runSpeed = 55f;
    public float dashForce = 10f;
    public float dashAccelerationTime = 1f;
    public float jumpForce = 7f;

    private float dashSmoothRef;
    public int moveDir = 0;
    public bool isGrounded = true;
    private bool isDashing = false;

    public bool canDash = true;

    public void Initialize(PlayerController c)
    {
        controller = c;
        animator = c.animator;
        rb = c.rb;
    }

    public void HandleMovement()
    {
        if (isGrounded && !controller.combat.isDefending)
        {
            float targetSpeed = moveDir != 0
                ? (animator.GetFloat("MoveSpeed") >= 3f ? runSpeed : walkSpeed) * moveDir
                : 0f;

            float newX = Mathf.SmoothDamp(rb.linearVelocity.x, targetSpeed, ref dashSmoothRef, dashAccelerationTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }

        runEffect.SetActive(animator.GetFloat("MoveSpeed") >= 3f);
    }

    public void SetDirection(int dir, float swipeMag)
    {
        if (canDash && !isDashing && controller.combat.canPadding && !isGrounded)
        {
            canDash = false;
            isDashing = true;
            animator.SetTrigger("JumpDash");
            rb.linearVelocity = new Vector2(dashForce * animator.GetInteger("Direction"), rb.linearVelocity.y);
            GameObject effect = Instantiate(dashEffect, transform);
            effect.transform.localPosition = new Vector3(0, -0.22f, 0);
            effect.transform.localEulerAngles  = new Vector3(0, 0, 90 * dir);

            return;
        }

        moveDir = dir;

        UpdateRunEffectDirection();

        float swipeRatio = swipeMag / Screen.width;
        float speedValue = 0f;

        if (swipeRatio <= 0.05f)
            speedValue = 0f;
        else if (swipeRatio <= 0.6f)
            speedValue = 1.5f;
        else
        {
            speedValue = 3f;
            isDashing = true;
        }


        animator.SetFloat("MoveSpeed", speedValue);
        animator.SetInteger("Direction", dir);
    }

    private void UpdateRunEffectDirection()
    {
        int moveDir = animator.GetInteger("Direction");
        Vector3 pos = runEffect.transform.localPosition;
        pos.x = 2.5f * (moveDir < 1 ? 1 : -1);
        runEffect.transform.localPosition = pos;

        runEffect.transform.localEulerAngles = new Vector3(
            runEffect.transform.localEulerAngles.x,
            90 * -moveDir,
            runEffect.transform.localEulerAngles.z
        );
    }

    public void Jump()
    {
        if (!isGrounded) return;
        canDash = true;
        isGrounded = false;
        animator.SetBool("IsGrounded", false);
        animator.SetTrigger("Jump");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public void Stop()
    {
        moveDir = 0;
        animator.SetFloat("MoveSpeed", 0f);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            animator.SetBool("IsGrounded", true);
            isDashing = false;
            controller.combat.canPadding = true;

            GameObject effect = Instantiate(groundHitEffect, transform);
            effect.transform.localPosition = new Vector3(0, -0.65f, 0);
        }
    }
}