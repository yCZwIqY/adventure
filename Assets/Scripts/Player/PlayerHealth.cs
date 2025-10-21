using System;
using Unity.Mathematics.Geometry;
using UnityEngine;
using Math = System.Math;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5;
    public float health = 5;
    private PlayerController pc;

    public bool isDead = false;
    public bool isImmune = false;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    public void TakeDamage(float damage, int dir, float knockbackPower = 1)
    {
        if (isImmune) return;
        health -= damage;
        if (health <= 0)
        {
            Die();
            return;
        }

        isImmune = true;
        ApplyKnockback(dir, knockbackPower);
        pc.animator.SetTrigger("Hit");
        Invoke(nameof(OffImmune), 1f);
    }

    public void OffImmune()
    {
        isImmune = false;
    }

    private void ApplyKnockback(int dir, float power)
    {
    }

    public void Heal(int heal)
    {
        health = Math.Max(maxHealth, heal + health);
    }

    public void UpgradeHealth()
    {
        maxHealth += 1;
    }

    public void Die()
    {
        isDead = true;
        pc.animator.SetTrigger("Death");
        pc.UnsubscribeInputEvent();
    }
}