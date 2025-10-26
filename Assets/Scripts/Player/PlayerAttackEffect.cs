using System;
using UnityEngine;

public class PlayerAttackEffect : MonoBehaviour
{
    private PlayerCombat playerCombat;

    private bool once = true;

    private void Start()
    {
        once = true;
        playerCombat = GetComponentInParent<PlayerCombat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!once) return;
        once = false;

        if (other.gameObject.CompareTag("Enemy"))
        {
            if (playerCombat.isParrying) playerCombat.Parrying();
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.TakeDamage(playerCombat.power, playerCombat.knockbackPower);
        }
        
        Debug.Log(other.gameObject.tag);

        if (other.gameObject.CompareTag("Indestructible"))
        {
            IndestructibleBlock block = other.GetComponent<IndestructibleBlock>();
            block.TakeDamage(playerCombat.power);
        }
    }
}