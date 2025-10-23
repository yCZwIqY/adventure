using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public PlayerController player;

    public GameObject healthContainer;
    public GameObject heart;
    public GameObject emptyHeart;

    private void Awake()
    {
        if (instance) Destroy(this);
        else instance = this;
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
}