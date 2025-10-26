using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // 게임 시작 버튼
    public void StartGame()
    {
        GameData gameData = SaveManager.Load();
        Debug.Log(gameData.playerPosition[0]);
        Vector3 playerPosition = new Vector3(gameData.playerPosition[0], gameData.playerPosition[1],
            gameData.playerPosition[2]);
       
        SceneTransitionManager.instance.LoadScene(gameData.lastSceneName, playerPosition);
    }

    // 게임 종료 버튼
    public void QuitGame()
    {
        // 에디터에선 종료 안 되므로 로그로 확인
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}