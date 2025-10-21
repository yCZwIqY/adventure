using System;
using UnityEngine;

public class PlayerAttackEffect : MonoBehaviour
{
    private PlayerCombat playerCombat;

    private void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (playerCombat.isParrying) playerCombat.Parrying();
            
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.TakeDamage(playerCombat.power, playerCombat.knockbackPower);
        }

    
    }
}