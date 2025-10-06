using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("기본 능력치")]
    public float maxHealth = 10f;
    public float health = 10f;
    public int damage = 1;

    [Header("피격/넉백 관련")]
    public float knockbackForce = 5f;
    public float recoilForce = 7f;
    public float playerAttackKnockbackForce = 3f;

    [Header("이동 관련")]
    public float patrolSpeed = 2f;
    public float patrolRange = 3f;
    public float chaseSpeed = 3.5f;
    public float detectionRange = 5f;

    [Header("공격 관련")]
    public float attackRange = 1f;
    public float attackPreDelay = 0.5f;     // 공격 전 준비
    public float attackPostDelay = 0.5f;    // 공격 후 경직
    public float attackCooldown = 3f;       // 다음 공격 대기시간

    [HideInInspector] public bool isAttacking = false;

    private bool isAttackCoolingDown = false;
    private bool isWaitingToAttack = false;

    public Rigidbody2D rb;
    protected Vector2 startPos;
    protected bool chasing = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    protected virtual void Update()
    {
        Vector2 playerPos = GetPlayerPosition();
        float distToPlayer = Vector2.Distance(playerPos, transform.position);

        // 추적 여부 판단
        if (distToPlayer <= detectionRange)
        {
            chasing = true;
        }
        else if (chasing && distToPlayer > detectionRange * 1.5f)
        {
            chasing = false;
        }

        // 공격, 대기, 쿨타임 중일 땐 정지
        if (isAttacking || isWaitingToAttack || isAttackCoolingDown)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 행동 결정
        if (chasing)
        {
            if (distToPlayer <= attackRange)
            {
                StartCoroutine(AttackRoutine());
            }
            else
            {
                ChasePlayer(playerPos);
            }
        }
        else
        {
            Patrol();
        }
    }

    protected virtual IEnumerator AttackRoutine()
    {
        isWaitingToAttack = true;
        rb.linearVelocity = Vector2.zero;

        // 1. 공격 준비 시간
        yield return new WaitForSeconds(attackPreDelay);
        isWaitingToAttack = false;

        // 2. 공격 실행
        isAttacking = true;
        Attack();
        yield return new WaitForSeconds(attackPostDelay);

        // 3. 공격 종료
        ResetAfterAttack();
        isAttacking = false;

        // 4. 쿨타임
        isAttackCoolingDown = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttackCoolingDown = false;
    }

    protected virtual void Patrol()
    {
        float patrolTargetX = Mathf.PingPong(Time.time * patrolSpeed, patrolRange) - patrolRange / 2f;
        Vector2 targetPos = new Vector2(startPos.x + patrolTargetX, transform.position.y);
        rb.MovePosition(Vector2.Lerp(transform.position, targetPos, Time.deltaTime * patrolSpeed));
    }

    protected virtual void ChasePlayer(Vector2 playerPos)
    {
        Vector2 dir = (playerPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * chaseSpeed, 0f);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                Vector2 dir = (player.transform.position - transform.position).normalized;
                Debug.Log($"{gameObject.name} 닿음!: {player.isDefending}");

                if (player.isDefending)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(new Vector2(10f, 10f), ForceMode2D.Impulse);
                }
                else
                {
                    Debug.Log($"{gameObject.name} 닿음2!");
                    player.OnHit(damage);
                    player.ApplyKnockback(dir, knockbackForce);
                }
            }
        }
    }

    public virtual void OnHit(float dmg)
    {
        health -= dmg;
        if (health <= 0)
            Die();
    }

    public virtual void ApplyKnockback(Vector2 direction)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * playerAttackKnockbackForce, ForceMode2D.Impulse);
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected Vector2 GetPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            return player.transform.position;
        return transform.position;
    }
    
    protected virtual void Attack()
    {
       return;
    }

    protected virtual void ResetAfterAttack()
    {
        return;
    }
}
