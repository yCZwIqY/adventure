using UnityEngine;

public class CollectibleItem : PersistentObject
{
    public void Collect()
    {
        RuntimeSceneState.Instance.MarkCollected(gameObject.scene.name, uniqueID);
        gameObject.SetActive(false);
    }
}
