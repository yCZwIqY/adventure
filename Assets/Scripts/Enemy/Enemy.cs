using System;
using UnityEngine;

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

    private Animator animator;
    private Vector3 startPosition;
    private bool movingRight = true;
    private float patrolTimer = 0f;
    private float patrolWaitTime = 2f;
    private bool isWaiting = false;
    private float lastAttackTime;

    [Header("Wall Detection")] public float wallCheckDistance = 1f;
    public LayerMask wallLayer;

    protected void Start()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        health = maxHealth;

        // 플레이어 자동 찾기
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void Update()
    {
        if (player == null) return;

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
        // 이동 멈춤
        Vector3 currentPos = transform.position;
        transform.position = currentPos;

        // 플레이어 방향 보기
        LookAtPlayer();

        if (!isCoolDown)
        {
            // 공격 애니메이션 실행
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            int direction = transform.rotation.eulerAngles.y > 90 ? 1 : -1;
            player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage, direction, knockbackPower);

            isCoolDown = true;
            lastAttackTime = Time.time;

            Debug.Log($"{gameObject.name} attacked!");
        }
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
            animator.SetBool("IsWaiting", isWaiting);
        }
    }

    public virtual void Chase()
    {
        // 플레이어 방향 계산
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

    private void LookAtPlayer()
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

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // 넉백 적용
        if (player != null)
        {
            // 플레이어 반대 방향으로 넉백
            Vector3 knockbackDir = (transform.position - player.transform.position).normalized;
            ApplyKnockback(knockbackDir, knockbackPower);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void ApplyKnockback(Vector3 direction, float power)
    {
    }

    protected virtual void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // 충돌 비활성화 (3D Collider)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 스크립트 비활성화
        this.enabled = false;

        // 2초 후 오브젝트 제거
        Destroy(gameObject, 2f);
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