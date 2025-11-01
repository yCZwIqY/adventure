using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // 게임 시작 버튼
    public void StartGame()
    {
        GameData gameData = SaveManager.Load();
        Debug.Log(gameData.playerPosition);
        Vector3 playerPosition = new Vector3(gameData.playerPosition[0], gameData.playerPosition[1],
            gameData.playerPosition[2]);

        SceneTransitionManager.instance.LoadScene(gameData.lastSceneName, playerPosition);
    }

    // 게임 종료 버튼
    public void QuitGame()
    {
        AlertManager.Instance.ShowInfoAlert("정말 종료하시겠습니까?", () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        });
        // 에디터에선 종료 안 되므로 로그로 확인
    }
}