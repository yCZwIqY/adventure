using System;
using UnityEngine;

public class NextScenePoint : MonoBehaviour
{
    public String sceneName;
    public Vector3 playerPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneTransitionManager.instance.LoadScene(sceneName, playerPosition);
        }
    }
}
