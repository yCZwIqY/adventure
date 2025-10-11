using System;
using UnityEngine;

public class Slime : Enemy
{
    [Header("슬라임 전용 설정")] public Animator animator;
    public GameObject attackTriggerObj;
    private CircleCollider2D attackCollider;

    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        attackCollider = attackTriggerObj.GetComponent<CircleCollider2D>();
    }

    public void LateUpdate()
    {
        attackTriggerObj.transform.localPosition = new Vector3(spriteRenderer.flipX ? 0.3f : -0.3f, 0, 0);
    }

    protected override void Attack()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger("Attack");

        // 공격 타이밍에 맞춰 트리거 활성화
        Invoke(nameof(EnableAttackCollider), 0.3f);
    }

    private void EnableAttackCollider()
    {
        attackCollider.enabled = false; // 완전히 껐다가
        attackCollider.enabled = true; // 즉시 다시 켜서 재인식 유도
        Invoke(nameof(DisableAttackCollider), 0.2f);
    }

    private void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    protected override void ResetAfterAttack()
    {
        isAttacking = false;
    }
}