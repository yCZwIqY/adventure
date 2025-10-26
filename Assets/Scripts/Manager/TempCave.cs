using System;
using UnityEngine;

public class TempCave : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(Return), 2f);
    }

    public void Return()
    {
        SceneTransitionManager.instance.LoadScene("Forest", new Vector3(74, -82, 0));
    }
}