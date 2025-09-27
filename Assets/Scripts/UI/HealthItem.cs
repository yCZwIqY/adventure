using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public GameObject fill;
    public GameObject empty;

    public void Toggle(bool toggle)
    {
        fill.SetActive(toggle);
        empty.SetActive(!toggle);
    }
}
