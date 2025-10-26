using System.Collections;
using UnityEngine;
using Math = System.Math;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5;
    public float health = 5;
    private PlayerController pc;

    public bool isDead = false;
    public bool isImmune = false;

    public GameObject hitEffect;
    protected Rigidbody rb;

    private Renderer[] renderers;
    [SerializeField] private float immuneDuration = 1f;
    [SerializeField] private float blinkInterval = 0.1f;

    public AudioClip hitSound;
    public AudioClip defenseHitSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
        renderers = GetComponentsInChildren<Renderer>();

        maxHealth = pc.gameData.playerMaxHealth;
        health = pc.gameData.playerHealth;

        UIManager.instance.RenderHealthUI();
    }

    public void TakeDamage(float damage, int dir, float knockbackPower = 1)
    {
        if (isImmune) return;
        health -= damage;
        Instantiate(hitEffect, transform);
        UIManager.instance.RenderHealthUI();

        if (health <= 0)
        {
            Die();
            return;
        }

        isImmune = true;
        ApplyKnockback(dir, knockbackPower);
        pc.animator.SetTrigger("Hit");
        if (SFXManager.instance != null)
        {
            if (pc.playerCombat.isDefend)
            {
                SFXManager.instance.PlaySFX(defenseHitSound);
            }
            else
            {
                SFXManager.instance.PlaySFX(hitSound);
            }
        }


        StartCoroutine(BlinkDuringImmune());
    }

    private IEnumerator BlinkDuringImmune()
    {
        float elapsed = 0f;

        // 각 렌더러의 원래 색 저장 (Material 인스턴스 복사)
        Color[] originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;

        while (elapsed < immuneDuration)
        {
            // 흰색으로 깜빡
            foreach (var r in renderers)
                r.material.color = Color.white;

            yield return new WaitForSeconds(blinkInterval);

            // 원래 색으로 복귀
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material.color = originalColors[i];

            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval * 2f;
        }

        // 마지막 복원
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];

        isImmune = false;
    }

    private void ApplyKnockback(int dir, float power)
    {
        Vector3 knockback = new Vector3(dir * power, 2f, 0f);
        rb.AddForce(knockback, ForceMode.Impulse);
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