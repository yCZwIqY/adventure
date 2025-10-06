using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyAttackTrigger : MonoBehaviour
{
    public Enemy enemy; // 부모 Enemy 참조

    private void Start()
    {
        // 자동으로 부모 Enemy 연결 (Inspector에서도 수동 연결 가능)
        if (enemy == null)
            enemy = GetComponentInParent<Enemy>();

        // Collider 설정 강제
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enemy.isAttacking) return; 

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player == null) return;

            Vector2 dir = (player.transform.position - enemy.transform.position).normalized;

            if (player.isDefending)
            {
                Debug.Log("공격 막힘!");
                enemy.rb.linearVelocity = Vector2.zero;
                enemy.rb.AddForce(-dir * enemy.recoilForce, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("플레이어 피격!");
                player.OnHit(enemy.damage);
                player.ApplyKnockback(dir, enemy.knockbackForce);
            }
        }
    }
}