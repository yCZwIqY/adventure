using System;
using UnityEngine;

public class SettingUI : MonoBehaviour
{
    
    public void OpenSetting()
    {
        gameObject.SetActive(true);
    }

    public void CloseSetting()
    {
        gameObject.SetActive(false);
    }
    
    public void Reset()
    {
        GameData data = SaveManager.Load();
        GameData resetData = new GameData();

        resetData.masterVolume = data.masterVolume;
        resetData.bgmVolume = data.bgmVolume;
        resetData.sfxVolume = data.sfxVolume;

        SaveManager.Save(resetData);
    }
}