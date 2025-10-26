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
    public AudioClip deadSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
        renderers = GetComponentsInChildren<Renderer>();

        maxHealth = pc.gameData.playerMaxHealth;
        health = pc.gameData.playerHealth;

        UIManager.instance.RenderHealthUI(maxHealth, health);
    }

    public void TakeDamage(float damage, int dir, float knockbackPower = 1)
    {
        if (isImmune) return;
        health -= damage;
        Instantiate(hitEffect, transform);
        UIManager.instance.RenderHealthUI(maxHealth, health);

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
        pc.animator.SetBool("IsDead", true);
        SFXManager.instance.PlaySFX(deadSound);
        pc.UnsubscribeInputEvent();
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // 1. 페이드 아웃
        yield return StartCoroutine(ScreenFader.instance.FadeOut());

        // 2. 데이터 초기화
        float[] respawnPoint = pc.gameData.playerRespawnPoint;
        GameData newGameData = new GameData(pc.gameData);

        newGameData.coin = 0;
        newGameData.lostCoin = pc.playerWallet.coin;
        newGameData.playerDeathPoint = new[] { transform.position.x, transform.position.y, transform.position.z };
        newGameData.playerPosition = respawnPoint;
        newGameData.playerRespawnPoint = respawnPoint;

        transform.position = new Vector3(
            respawnPoint[0],
            respawnPoint[1],
            respawnPoint[2]);

        health = maxHealth;
        pc.playerWallet.coin = 0;

        // 3. 잠시 대기
        yield return new WaitForSeconds(0.2f);

        // 4. 페이드 인
        yield return StartCoroutine(ScreenFader.instance.FadeIn());

        // 5. 입력 재활성화
        isDead = false;
        pc.animator.SetBool("IsDead", false);
        UIManager.instance.RenderHealthUI(maxHealth, health);
        UIManager.instance.RenderCoinUI(0);
        pc.SubscribeInputEvent();
        SaveManager.Save(newGameData);
        SceneStateManager.instance.SaveCurrentState();
        pc.playerWallet.GeneratePocket(newGameData);
    }
}