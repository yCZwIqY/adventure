using System;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    private PlayerController pc;
    public int coin = 0;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
        coin = pc.gameData.coin;
        UIManager.instance.RenderCoinUI(coin);
    }

    public void AddCoin(int coin)
    {
        this.coin += coin;
        UIManager.instance.RenderCoinUI(coin);
    }
}