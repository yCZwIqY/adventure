using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateRestorer : MonoBehaviour
{
    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        var data = SaveManager.Load();
        var persistentObjects = FindObjectsOfType<PersistentObject>(true);

        foreach (var obj in persistentObjects)
        {
            if (data.destroyedObjects.ToHashSet().Contains(obj.uniqueID))
                Destroy(obj.gameObject);
            else if (data.collectedItems.ToHashSet().Contains(obj.uniqueID))
                obj.gameObject.SetActive(false);
        }
    }
}
