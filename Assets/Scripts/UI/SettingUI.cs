using System;
using UnityEngine;

public class SettingUI : MonoBehaviour
{
    public GameObject overlay;

    public void OpenSetting()
    {
        overlay.SetActive(true);
    }

    public void CloseSetting()
    {
        overlay.SetActive(false);
    }

    public void Reset()
    {
        AlertManager.Instance.ShowWarningAlert("모든 게임 데이터가 초기화됩니다. 진행하시겠습니까?", () =>
        {
            GameData data = SaveManager.Load();
            GameData resetData = new GameData();

            resetData.masterVolume = data.masterVolume;
            resetData.bgmVolume = data.bgmVolume;
            resetData.sfxVolume = data.sfxVolume;

            SaveManager.Save(resetData);
            
            AlertManager.Instance.ShowAutoCloseInfoAlert("초기화 완료.");
        });
    }
}