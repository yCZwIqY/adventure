using UnityEngine;

public class Slime : Enemy
{
    public override void Attack()
    {
        // 이동 멈춤
        Vector3 currentPos = transform.position;
        transform.position = currentPos;

        // 플레이어 방향 보기
        LookAtPlayer();

        if (!isCoolDown)
        {
            enemySfx.PlayAttack();
            animator.SetTrigger("Attack");
            isCoolDown = true;
            lastAttackTime = Time.time;

            Invoke(nameof(DelayedAttackBase), 0.3f);
        }
    }
}