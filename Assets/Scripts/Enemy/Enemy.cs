using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("기본 능력치")] public float maxHealth = 10f;
    public float health = 10f;
    public int damage = 1;

    [Header("Sprite & Direction")] public SpriteRenderer spriteRenderer;

    [Header("피격/넉백 관련")] public float knockbackForce = 5f;
    public float recoilForce = 7f;
    public float playerAttackKnockbackForce = 3f;
    public bool isRecoiling = false;

    [Header("이동 관련")] public float patrolSpeed = 2f;
    public float patrolRange = 3f;
    public float chaseSpeed = 3.5f;
    public float detectionRange = 5f;

    [Header("공격 관련")] public float attackRange = 1f;
    public float attackPreDelay = 0.5f; // 공격 전 준비
    public float attackPostDelay = 0.5f; // 공격 후 경직
    public float attackCooldown = 3f; // 다음 공격 대기시간

    [HideInInspector] public bool isAttacking = false;

    private bool isAttackCoolingDown = false;
    private bool isWaitingToAttack = false;

    public Rigidbody2D rb;
    protected Vector2 startPos;
    public bool chasing = false;

    enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    EnemyState currentState = EnemyState.Patrol;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        Vector2 playerPos = GetPlayerPosition();
        float distToPlayer = Math.Abs(Vector2.Distance(playerPos, transform.position));

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = playerPos.x > transform.position.x;
        }


        // 공격, 대기, 쿨타임 중일 땐 정지
        if ((isAttacking || isWaitingToAttack) && !isRecoiling)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                if (distToPlayer <= detectionRange)
                    currentState = EnemyState.Chase;
                Patrol();
                break;

            case EnemyState.Chase:
                if (distToPlayer <= attackRange)
                    currentState = EnemyState.Attack;
                else if (distToPlayer > detectionRange)
                    currentState = EnemyState.Patrol;
                else if(!isRecoiling)
                    ChasePlayer(GetPlayerPosition());
                break;

            case EnemyState.Attack:
                if (distToPlayer > attackRange * 1.2f)
                    currentState = EnemyState.Chase;
                else
                {
                    if (!isAttacking && !isAttackCoolingDown)
                        StartCoroutine(AttackRoutine());
                }

                break;
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


    protected virtual void OnTriggerEnter2D(Collision2D collision)
    {
        rb.linearVelocity = Vector3.zero;
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController == null) return;

            PlayerCombat combat = playerController.combat;
            PlayerHealth playerHealth = playerController.health;

            collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            Vector2 dir = (player.transform.position - transform.position).normalized;
            Debug.Log($"{gameObject.name} 닿음!: {combat.isDefending}");

            if (combat.isDefending)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(new Vector2(10f, 10f), ForceMode2D.Impulse);
            }
            else
            {
                Rigidbody2D playerRb = playerController.rb;
                playerHealth.OnHit(damage);
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
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
    
    public void ApplyRecoil(Vector2 dir, float force)
    {
        if (isRecoiling) return;
        StartCoroutine(RecoilCoroutine(dir, force));
    }

    private IEnumerator RecoilCoroutine(Vector2 dir, float force)
    {
        isRecoiling = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.5f); // 반동 지속시간
        isRecoiling = false;
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