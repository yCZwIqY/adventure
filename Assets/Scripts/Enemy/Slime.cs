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
            animator.SetTrigger("Attack");
            isCoolDown = true;
            lastAttackTime = Time.time;
            
            Invoke(nameof(DelayedAttackBase), 0.3f);
        }
    }

    private void DelayedAttackBase()
    {
        int direction = transform.rotation.eulerAngles.y > 90 ? 1 : -1;
        player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage, direction, knockbackPower);
    }
}