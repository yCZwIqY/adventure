using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    public string uniqueID;

    private void Awake()
    {
        // 혹시 비어 있다면 자동으로 하나 만들어줌
        if (string.IsNullOrEmpty(uniqueID))
            uniqueID = $"{gameObject.scene.name}_{gameObject.name}_{transform.position}";
    }
}