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
                AlertManager.Instance.ShowInfoAlert("정말 종료하시겠습니까?", () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
                });
            }
        }
    }
}