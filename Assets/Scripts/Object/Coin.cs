using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("코인 먹음");
            PlayerWallet wallet = other.GetComponent<PlayerWallet>();
            wallet.AddCoin(amount);
            Destroy(this.gameObject);
        }
    }
}