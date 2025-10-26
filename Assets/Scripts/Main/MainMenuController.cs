using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // 게임 시작 버튼
    public void StartGame()
    {
        // "GameScene"은 실제 게임이 들어있는 씬 이름으로 바꿔주세요
        SceneManager.LoadScene("Forest");
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