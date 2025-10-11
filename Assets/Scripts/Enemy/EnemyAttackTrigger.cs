using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyAttackTrigger : MonoBehaviour
{
    public Enemy enemy; // 부모 Enemy 참조

    private void Start()
    {
        if (enemy == null)
            enemy = GetComponentInParent<Enemy>();

        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enemy.isAttacking) return;

        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController == null) return;

            var combat = playerController.combat;
            var health = playerController.health;
            var playerRb = playerController.rb;

            Vector2 dir = (playerController.transform.position - enemy.transform.position).normalized;

            if (combat != null && combat.isDefending)
            {
                if (enemy.rb != null)
                {
                    enemy.rb.linearVelocity = Vector2.zero;
                    enemy.ApplyRecoil(-dir, enemy.recoilForce);
                    playerController.combat.Defend();
                }
            }
            else
            {
                Debug.Log("플레이어 피격!");
                if (health != null)
                    health.OnHit(enemy.damage);

                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector2.zero;
                    playerRb.AddForce(dir * enemy.knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}