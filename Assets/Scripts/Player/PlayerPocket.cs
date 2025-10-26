using System;
using UnityEngine;

public class PlayerPocket : MonoBehaviour
{
    public int coin;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerWallet wallet = other.GetComponent<PlayerWallet>();
            wallet.coin = coin;
            UIManager.instance.RenderCoinUI(wallet.coin);
            Destroy(gameObject);
        }
    }
}