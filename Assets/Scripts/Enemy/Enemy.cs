using System;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Header("References")] public GameObject player;

    [Header("Stats")] public float maxHealth = 100f;
    public float health = 100f;
    public float speed = 3f;
    public float attackDamage = 1f;
    public float knockbackPower = 1f;

    [Header("Distances")] public float detectionDistance = 10f;
    public float attackDistance = 2f;
    public float patrolDistance = 5f;

    [Header("Combat")] public float attackCoolTime = 1.5f;
    public bool isCoolDown = false;
    public bool isChase = false;
    public bool canAttack = true;

    protected Animator animator;
    protected Vector3 startPosition;
    protected bool movingRight = true;
    protected float patrolTimer = 0f;
    protected float patrolWaitTime = 2f;
    protected bool isWaiting = false;
    protected float lastAttackTime;
    protected Rigidbody rb;

    protected PlayerController pc;

    protected EnemySFX enemySfx;


    public int coin = 1;
    public GameObject coinPrefab;

    [Header("Wall Detection")] public float wallCheckDistance = 1f;
    public LayerMask wallLayer;

    protected void Start()
    {
        enemySfx = GetComponent<EnemySFX>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        health = maxHealth;

        isCoolDown = false;
        isChase = false;
        canAttack = true;

        // 플레이어 자동 찾기
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        pc = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player == null) return;

        if (player.GetComponent<PlayerHealth>().isDead)
        {
            Patrol();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Debug Ray 그리기 (Update에서만)
        int direction = transform.rotation.eulerAngles.y > 90 ? -1 : 1;
        Debug.DrawRay(transform.position, Vector3.right * direction * wallCheckDistance, Color.red);

        // 상태 결정
        if (distanceToPlayer <= attackDistance)
        {
            // 공격 범위 내
            Attack();
        }
        else if (distanceToPlayer <= detectionDistance)
        {
            if (!isCoolDown)
            {
                // 감지 범위 내 - 추적
                isChase = true;
                Chase();
            }
        }
        else
        {
            // 감지 범위 밖 - 배회
            isChase = false;
            Patrol();
        }

        // 쿨다운 체크
        if (isCoolDown && Time.time >= lastAttackTime + attackCoolTime)
        {
            isCoolDown = false;
        }
    }

    public virtual void Attack()
    {
    }

    public virtual void DelayedAttackBase()
    {
        if (!canAttack) return;

        int direction = transform.rotation.eulerAngles.y > 90 ? 1 : -1;
        if (pc.playerCombat.isDefend)
        {
            ApplyKnockback(direction * -1, pc.playerCombat.defensePower);
            pc.animator.SetTrigger("Hit");
            return;
        }

        pc.playerHealth?.TakeDamage(attackDamage, direction, knockbackPower);
    }

    public virtual void Patrol()
    {
        if (isWaiting)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                isWaiting = false;
                animator.SetBool("IsWaiting", isWaiting);
                patrolTimer = 0f;
                movingRight = !movingRight; // 방향 전환
            }

            return;
        }
        
        enemySfx.PlayPatrol();

        // 방향 전환 (이동 전에 설정)
        FlipDirection(movingRight);

        // 앞에 벽이 있는지 체크
        if (IsWallAhead())
        {
            // 벽을 만나면 즉시 방향 전환
            movingRight = !movingRight;
        }

        // 배회 이동
        float targetX = movingRight ? startPosition.x + patrolDistance : startPosition.x - patrolDistance;

        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        Vector3 move = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        ) - transform.position;

        // 이동 적용
        transform.position += move;

        // 목표 지점 도착 시 대기
        if (Mathf.Abs(transform.position.x - targetX) < 0.1f)
        {
            isWaiting = true;
            enemySfx.StopLoop();
            animator.SetBool("IsWaiting", isWaiting);
        }
    }

    public virtual void Chase()
    {
        // 플레이어 방향 계산
        enemySfx.PlayChase();

        Vector3 direction = (player.transform.position - transform.position).normalized;
        int dir = direction.x > 0 ? 1 : -1;

        // 방향 설정
        FlipDirection(direction.x > 0);

        // 앞에 벽이 있으면 추적 중단
        if (IsWallAhead())
        {
            return;
        }

        // 플레이어 방향으로 이동 (X축만)
        Vector3 move = new Vector3(dir * speed * Time.deltaTime, 0, 0);
        transform.position += move;
    }

    protected void LookAtPlayer()
    {
        float direction = player.transform.position.x - transform.position.x;
        FlipDirection(direction > 0);
    }

    private void FlipDirection(bool faceRight)
    {
        // 3D - Y축 회전으로 방향 전환
        transform.rotation = Quaternion.Euler(0, faceRight ? 180 : 0, 0);
    }

    private bool IsWallAhead()
    {
        // 현재 바라보는 방향 계산 (X축 기준)
        int direction = transform.rotation.eulerAngles.y > 90 ? 1 : -1;

        // Raycast로 벽 감지
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.right * direction, out hit, wallCheckDistance))
        {
            // Debug용 Ray는 Update에서만 그리기
            if (hit.transform != null && hit.transform.CompareTag("Wall"))
            {
                return true;
            }
        }

        return false;
    }

    public virtual void TakeDamage(float damage, float knockbackPower = 1)
    {
        health -= damage;
        canAttack = false;
        Invoke(nameof(OnAttack), 0.5f);

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        enemySfx.PlayHit();

        if (player != null)
        {
            int dir = transform.rotation.eulerAngles.y > 90 ? -1 : 1;
            ApplyKnockback(dir, knockbackPower);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public void OnAttack()
    {
        canAttack = true;
    }

    public void ApplyKnockback(float dir, float power)
    {
        Vector3 knockback = new Vector3(dir * power, 2f, 0f);
        rb.AddForce(knockback, ForceMode.Impulse);
    }

    protected virtual void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        enemySfx.PlayDie();

        // 충돌 비활성화 (3D Collider)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 코인 드롭
        Invoke(nameof(DropCoins), 0.3f);

        // 스크립트 비활성화
        this.enabled = false;

        // 2초 후 오브젝트 제거
        Destroy(gameObject, 2f);
    }

    private void DropCoins()
    {
        for (int i = 0; i < coin; i++)
        {
            // 코인 생성
            GameObject coinObj = Instantiate(coinPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);

            Rigidbody rb = coinObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 💨 X축으로 살짝, Y축으로 크게 튀어오르기 (Z는 고정)
                Vector3 randomDir = new Vector3(
                    UnityEngine.Random.Range(-3f, 3f), // 좌우 흩어짐
                    UnityEngine.Random.Range(0.8f, 1.2f), // 위로 튀어오름
                    0f // Z축 없음
                );
                rb.AddForce(randomDir, ForceMode.Impulse);
            }
        }
    }


    // 디버그용 시각화
    private void OnDrawGizmosSelected()
    {
        // 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);

        // 공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        // 배회 범위
        Gizmos.color = Color.green;
        Vector3 startPos = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawLine(
            new Vector3(startPos.x - patrolDistance, startPos.y, startPos.z),
            new Vector3(startPos.x + patrolDistance, startPos.y, startPos.z)
        );

        // 벽 감지 범위 (X축 방향)
        Gizmos.color = Color.blue;
        int direction = Application.isPlaying ? (transform.rotation.eulerAngles.y > 90 ? -1 : 1) : 1;
        Gizmos.DrawRay(transform.position, Vector3.right * direction * wallCheckDistance);
    }
}