using System;

[Serializable]
public class GameData
{
    // 설정
    public float masterVolume = 1;
    public float bgmVolume = 1;
    public float sfxVolume = 1;

    //플레이 데이터
    public String lastSceneName = "Forest";
    public float[] playerPosition = new[] { -6f, 11f, 0 };
    public float playerMaxHealth = 3;
    public float playerHealth = 3;

    public bool isFirstPlay = true;
    public int coin = 0;
}