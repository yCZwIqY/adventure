using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;

    public int maxHealth = 5;
    public int currentHealth = 5;

    public void Initialize(PlayerController c)
    {
        controller = c;
        animator = c.animator;
    }

    public void Start()
    {
        UIManager.Instance.RenderPlayerHealth(currentHealth);
    }

    public void OnHit(int damage)
    {
        if (controller.combat.isDefending) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        animator.SetTrigger("Hit");

        UIManager.Instance.RenderPlayerHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player Dead");
        animator.SetTrigger("Die");
    }
}