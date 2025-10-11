using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCombat : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;
    private Rigidbody2D rb;

    public GameObject attackEffect;
    public GameObject dependEffect;
    public GameObject effect;
    public CircleCollider2D attackRange;

    public bool isAttack = false;
    public bool isDefending = false;
    public bool canPadding = false;
    public float attackPower = 1f;
    

    public void Initialize(PlayerController c)
    {
        controller = c;
        animator = c.animator;
        rb = c.rb;
        attackRange = GetComponent<CircleCollider2D>();
        attackRange.enabled = false;
    }

    public void Attack()
    {
        if (isAttack || !canPadding) return;

        if (!controller.movement.isGrounded && controller.movement.canDash)
        {
            canPadding = false;
            animator.SetTrigger("Padding");
        }
        else
        {
            animator.SetTrigger("Attack");
        }
     
        attackRange.enabled = true;
        isAttack = true;
        SpawnAttackEffect();

        Invoke(nameof(EndAttack), 0.5f);
    }

    private void EndAttack()
    {
        attackRange.enabled = false;
        isAttack = false;
        canPadding = true;
    }

    public void StartDefend()
    {
        if (isDefending) return;
        isDefending = true;
        animator.SetBool("IsDefense", true);
        rb.linearVelocity = Vector2.zero;
    }

    public void EndDefend()
    {
        if (!isDefending) return;
        isDefending = false;
        animator.SetBool("IsDefense", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttack && other.CompareTag("Enemy"))
        {
            Enemy e = other.GetComponent<Enemy>();
            if (e != null)
            {
                Vector2 dir = (e.transform.position - transform.position).normalized;
                e.OnHit(attackPower);
                e.ApplyKnockback(dir);
            }
        }

        if (!canPadding && (other.CompareTag("Enemy") || other.CompareTag("HitPoint")))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, controller.movement.jumpForce);
            controller.movement.canDash = true;
        }
    }

    private void SpawnAttackEffect()
    {
        Vector3 pos = effect.transform.position;
        int dir = animator.GetInteger("Direction");

        GameObject effectObj = Instantiate(attackEffect, pos, Quaternion.identity, effect.transform);
        
        if (dir == -1)
        {
            effectObj.transform.localRotation = Quaternion.Euler(0, 10f, 0);
        }
        else
        {
            effectObj.transform.localRotation = Quaternion.Euler(0, 170f, 0);
        }
    }

    public void Defend()
    {
        
    }
}