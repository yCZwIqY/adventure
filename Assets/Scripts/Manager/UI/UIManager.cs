using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public PlayerController player;

    public GameObject healthContainer;
    public GameObject heart;
    public GameObject emptyHeart;

    public GameObject tutorialPanel;

    public TMPro.TMP_Text coinText;

    private void Awake()
    {
        if (instance) Destroy(this);
        else instance = this;
    }

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        if (player.gameData.isFirstPlay)
        {
            tutorialPanel.SetActive(true);
        }
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        player.gameData.isFirstPlay = false;
        SaveManager.Save(player.gameData);
    }

    public void RenderHealthUI(float maxHealth, float currentHealth)
    {
        for (int i = 0; i < healthContainer.transform.childCount; i++)
        {
            Destroy(healthContainer.transform.GetChild(i).gameObject);
        }


        for (int i = 0; i < maxHealth; i++)
        {
            if (i < currentHealth)
            {
                Instantiate(heart, healthContainer.transform);
            }
            else
            {
                Instantiate(emptyHeart, healthContainer.transform);
            }
        }
    }

    public void RenderCoinUI(int coin)
    {
        coinText.SetText("Coin: " + coin.ToString());
    }
}