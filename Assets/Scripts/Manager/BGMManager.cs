using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public SceneBGMData bgmData;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        DontDestroyOnLoad(gameObject);

        // 씬 변경 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    private void PlayBGMForScene(string sceneName)
    {
        foreach (var entry in bgmData.sceneBGMs)
        {
            if (entry.sceneName == sceneName && entry.bgmClip != null)
            {
                if (audioSource.clip == entry.bgmClip && audioSource.isPlaying)
                    return;

                audioSource.clip = entry.bgmClip;
                audioSource.Play();
                return;
            }
        }

        // 해당 씬에 BGM이 없으면 정지
        audioSource.Stop();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}