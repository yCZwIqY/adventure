using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private void Start()
    {
        StartCoroutine(LoadNextSceneRoutine());
    }

    private IEnumerator LoadNextSceneRoutine()
    {
        yield return new WaitForSeconds(0.3f); // 살짝 연출용 대기

        string nextScene = SceneTransitionManager.instance.nextSceneName;

        // 실제 로드 시작
        AsyncOperation async = SceneManager.LoadSceneAsync(nextScene);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            if (async.progress >= 0.9f)
                async.allowSceneActivation = true;

            yield return null;
        }
    }
}