using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;

    [Header("Fade Controller")] public Animator fadeAnimator;
    public float minLoadTime = 1.0f;

    public string nextSceneName;
    public Vector3 playerPosition;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName, Vector3 playerPosition)
    {
        this.playerPosition = playerPosition;
        LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        nextSceneName = sceneName;
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("FadeOut");

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("LoadingScene");

        yield return new WaitForSeconds(0.2f);

        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;

        float elapsed = 0f;
        while (op.progress < 0.9f || elapsed < minLoadTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        op.allowSceneActivation = true;
    }
}