using System.Collections;
using UnityEngine;

public class Slime : Enemy
{
    [Header("슬라임 전용 설정")]
    public Animator animator;
    public CircleCollider2D attackCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (attackCollider == null)
            attackCollider = GetComponentInChildren<CircleCollider2D>();
    }

    // ✅ Enemy의 Attack()을 오버라이드
    protected override void Attack()
    {
        isAttacking = true;

        // 공격 애니메이션 트리거
        if (animator != null)
            animator.SetTrigger("Attack");

        // 공격 판정 Collider 활성화
        if (attackCollider != null)
            attackCollider.enabled = true;
        
    }

    // ✅ 부모의 ResetAfterAttack()을 오버라이드
    protected override void ResetAfterAttack()
    {

        // 공격 종료 후 Collider 끄기
        if (attackCollider != null)
            attackCollider.enabled = false;

        isAttacking = false;
    }
}