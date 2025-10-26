using System;

[Serializable]
public class GameData
{
    // 설정
    public float masterVolume = 1;
    public float bgmVolume = 1;
    public float sfxVolume = 1;

    // 플레이 데이터
    public string lastSceneName = "Forest";
    public float[] playerPosition = new[] { -6f, 11f, 0 };
    public float[] playerRespawnPoint = new[] { -6f, 11f, 0 };
    public float[] playerDeathPoint = new float[0];
    public float playerMaxHealth = 3;
    public float playerHealth = 3;

    public bool isFirstPlay = true;
    public int coin = 0;
    public int lostCoin = 0;

    // 기본 생성자
    public GameData()
    {
        masterVolume = 1;
        bgmVolume = 1;
        sfxVolume = 1;

        lastSceneName = "Forest";
        playerPosition = new[] { -6f, 11f, 0 };
        playerRespawnPoint = new[] { -6f, 11f, 0 };
        playerDeathPoint = new float[0];
        playerMaxHealth = 3;
        playerHealth = 3;
        isFirstPlay = true;
        coin = 0;
        lostCoin = 0;
    }

    // 복사 생성자 (깊은 복사)
    public GameData(GameData gameData)
    {
        masterVolume = gameData.masterVolume;
        bgmVolume = gameData.bgmVolume;
        sfxVolume = gameData.sfxVolume;

        lastSceneName = gameData.lastSceneName;

        // 배열은 새로 복사
        playerPosition = (float[])gameData.playerPosition.Clone();
        playerRespawnPoint = (float[])gameData.playerRespawnPoint.Clone();
        playerDeathPoint = (float[])gameData.playerDeathPoint.Clone();

        playerMaxHealth = gameData.playerMaxHealth;
        playerHealth = gameData.playerHealth;
        isFirstPlay = gameData.isFirstPlay;
        coin = gameData.coin;
        lostCoin = gameData.lostCoin;
    }
}