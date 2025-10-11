using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyAttackTrigger : MonoBehaviour
{
    public Enemy enemy; // 부모 Enemy 참조

    private void Start()
    {
        // 부모 Enemy 자동 연결
        if (enemy == null)
            enemy = GetComponentInParent<Enemy>();

        // Collider 설정
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enemy.isAttacking) return;

        if (other.CompareTag("Player"))
        {
            // Player의 세부 스크립트 가져오기
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController == null) return;

            PlayerCombat combat = playerController.combat;
            PlayerHealth health = playerController.health;
            Rigidbody2D playerRb = playerController.rb;

            Vector2 dir = (playerController.transform.position - enemy.transform.position).normalized;

            if (combat != null && combat.isDefending)
            {
                Debug.Log("공격 막힘!");
                if (enemy.rb != null)
                {
                    enemy.rb.linearVelocity = Vector2.zero;
                    enemy.rb.AddForce(-dir * enemy.recoilForce, ForceMode2D.Impulse);
                }
            }
            else
            {
                Debug.Log("플레이어 피격!");
                if (health != null)
                {
                    health.OnHit(enemy.damage);
                }

                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector2.zero;
                    playerRb.AddForce(dir * enemy.knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}