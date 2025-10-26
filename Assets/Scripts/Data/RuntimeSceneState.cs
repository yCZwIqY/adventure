using System.Collections.Generic;
using UnityEngine;

public class RuntimeSceneState
{
    // 씬 이름 → 파괴/획득 오브젝트 ID
    public Dictionary<string, HashSet<string>> destroyedObjects = new();
    public Dictionary<string, HashSet<string>> collectedItems = new();

    private static RuntimeSceneState _instance;
    public static RuntimeSceneState Instance => _instance ??= new RuntimeSceneState();

    public void MarkDestroyed(string sceneName, string id)
    {
        if (!destroyedObjects.ContainsKey(sceneName))
            destroyedObjects[sceneName] = new HashSet<string>();
        destroyedObjects[sceneName].Add(id);
    }

    public void MarkCollected(string sceneName, string id)
    {
        if (!collectedItems.ContainsKey(sceneName))
            collectedItems[sceneName] = new HashSet<string>();
        collectedItems[sceneName].Add(id);
    }

    public bool IsDestroyed(string sceneName, string id)
        => destroyedObjects.ContainsKey(sceneName) && destroyedObjects[sceneName].Contains(id);

    public bool IsCollected(string sceneName, string id)
        => collectedItems.ContainsKey(sceneName) && collectedItems[sceneName].Contains(id);
}