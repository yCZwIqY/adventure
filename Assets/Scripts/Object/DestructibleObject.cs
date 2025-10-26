using UnityEngine;

public class DestructibleObject : PersistentObject
{
    public void DestroyObject()
    {
        RuntimeSceneState.Instance.MarkDestroyed(gameObject.scene.name, uniqueID);
        Destroy(gameObject);
    }
}