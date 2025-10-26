using UnityEngine;

public class BackButtonExit : MonoBehaviour
{
    void Update()
    {
        // Android 뒤로가기 버튼
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("뒤로가기 눌림, 게임 종료");
                Application.Quit(); // 앱 종료
            }
        }
    }
}