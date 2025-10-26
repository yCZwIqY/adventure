using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyRuntimeState(scene.name);
        ApplySavedState(scene.name);
    }

    // 씬 이동 시 임시 상태 적용
    private void ApplyRuntimeState(string sceneName)
    {
        var objects = FindObjectsOfType<PersistentObject>(true);
        foreach (var obj in objects)
        {
            if (RuntimeSceneState.Instance.IsDestroyed(sceneName, obj.uniqueID))
                Destroy(obj.gameObject);
            else if (RuntimeSceneState.Instance.IsCollected(sceneName, obj.uniqueID))
                obj.gameObject.SetActive(false);
        }
    }

    // 저장된 상태 적용 (게임 재시작)
    private void ApplySavedState(string sceneName)
    {
        var data = SaveManager.Load();
        var objects = FindObjectsOfType<PersistentObject>(true);

        foreach (var obj in objects)
        {
            if (data.destroyedObjects.ToHashSet().Contains(obj.uniqueID))
                Destroy(obj.gameObject);
            else if (data.collectedItems.ToHashSet().Contains(obj.uniqueID))
                obj.gameObject.SetActive(false);
        }
    }

    // 영구 저장
    public void SaveCurrentState()
    {
        var data = SaveManager.Load();
        string sceneName = SceneManager.GetActiveScene().name;

        data.destroyedObjects = new SerializableHashSet(
            RuntimeSceneState.Instance.destroyedObjects.ContainsKey(sceneName) ?
            RuntimeSceneState.Instance.destroyedObjects[sceneName] :
            new HashSet<string>()
        );

        data.collectedItems = new SerializableHashSet(
            RuntimeSceneState.Instance.collectedItems.ContainsKey(sceneName) ?
            RuntimeSceneState.Instance.collectedItems[sceneName] :
            new HashSet<string>()
        );

        SaveManager.Save(data);
    }
}
