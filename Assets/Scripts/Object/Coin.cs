using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int amount = 1;
    public AudioClip getCoinSound;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SFXManager.instance.PlaySFX(getCoinSound);
            PlayerWallet wallet = other.GetComponent<PlayerWallet>();
            wallet.AddCoin(amount);
            Destroy(gameObject);
        }
    }
}