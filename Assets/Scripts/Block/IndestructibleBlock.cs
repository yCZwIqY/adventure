using UnityEngine;

public class IndestructibleBlock : MonoBehaviour
{
    public float health = 100f;
    public float shakeDuration = 0.2f; // 흔들리는 시간
    public float shakeMagnitude = 0.1f; // 흔들리는 강도
    public GameObject destroyEffect; // 부서질 때 이펙트

    private Vector3 originalPos;
    private bool isShaking = false;

    private void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log(name + " TakeDamage");
        health -= damage;
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }

        if (health <= 0)
        {
            Broke();
        }
    }

    private System.Collections.IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            randomOffset.y = 0; // 수평으로만 흔들리게
            transform.localPosition = originalPos + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        isShaking = false;
    }

    public void Broke()
    {
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}