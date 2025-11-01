using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWallet : MonoBehaviour
{
    private PlayerController pc;
    public int coin = 0;
    public GameObject playerPocket;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
        coin = pc.gameData.coin;
        UIManager.instance.RenderCoinUI(coin);

        GeneratePocket(pc.gameData);
    }

    public void GeneratePocket(GameData gameData)
    {
        PlayerPocket existingPocket = FindObjectOfType<PlayerPocket>();
        if (existingPocket != null)
        {
            Debug.Log("이미 PlayerPocket이 존재합니다. 새로 생성하지 않습니다.");
            return;
        }

        float[] deathPoint = pc.gameData.playerDeathPoint;
        if (SceneManager.GetActiveScene().name == pc.gameData.lastSceneName && deathPoint != null &&
            deathPoint.Length > 1 && pc.gameData.lostCoin > 0)
        {
            Vector3 spawnPos = new Vector3(deathPoint[0], deathPoint[1], 0f);
            GameObject pocket = Instantiate(playerPocket, spawnPos, Quaternion.identity);
            pocket.GetComponent<PlayerPocket>().coin = pc.gameData.lostCoin;
        }
    }

    public void AddCoin(int coin)
    {
        this.coin += coin;
        UIManager.instance.RenderCoinUI(this.coin);
    }

    public bool TrySpend(int price)
    {
        return coin - price >= 0;
    }
}