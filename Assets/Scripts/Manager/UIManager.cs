using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public List<HealthItem> healthItems;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void RenderUi() {}

    public void RenderPlayerHealth(int health)
    {
        for (int i = 0; i < healthItems.Count; i++)
        {
            healthItems[i].Toggle(i < health);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
