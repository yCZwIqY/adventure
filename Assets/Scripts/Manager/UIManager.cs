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

    public void RenderHealthUI()
    {
        for (int i = 0; i < healthContainer.transform.childCount; i++)
        {
            Destroy(healthContainer.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < player.playerHealth.maxHealth; i++)
        {
            if (i < player.playerHealth.health)
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