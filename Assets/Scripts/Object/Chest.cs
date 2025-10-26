using UnityEngine;

public class Chest : CollectibleItem
{
    public GameObject coinPrefab;
    public int amount = 10;
    public AudioClip openSound;

    public void Open()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Open");
        Invoke(nameof(DropCoins), 1f);
        Invoke(nameof(Collect), 3f);
    }
    
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
    private void DropCoins()
    {
        for (int i = 0; i < amount; i++)
        {
            // 코인 생성
            GameObject coinObj = Instantiate(coinPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);

            Rigidbody rb = coinObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 💨 X축으로 살짝, Y축으로 크게 튀어오르기 (Z는 고정)
                Vector3 randomDir = new Vector3(
                    UnityEngine.Random.Range(-3f, 3f), // 좌우 흩어짐
                    UnityEngine.Random.Range(1f, 1.5f), // 위로 튀어오름
                    0f // Z축 없음
                );
                rb.AddForce(randomDir, ForceMode.Impulse);
            }
        }
    }
}
