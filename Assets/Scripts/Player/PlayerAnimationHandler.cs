using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;
    private Rigidbody2D rb;

    [Range(0.05f, 1f)] public float animationSpeedMultiplier = 0.1f;
    public float minAnimSpeed = 0.5f;
    public float maxAnimSpeed = 2f;

    public void Initialize(PlayerController c)
    {
        controller = c;
        animator = c.animator;
        rb = c.rb;
    }

    public void UpdateAnimatorSpeed()
    {
        float target = Mathf.Clamp(Mathf.Abs(rb.linearVelocity.x) * animationSpeedMultiplier, minAnimSpeed, maxAnimSpeed);
        animator.speed = Mathf.Lerp(animator.speed, target, Time.deltaTime * 5f);
    }
}