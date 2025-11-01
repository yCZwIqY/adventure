using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    void Start()
    {
        if (SceneTransitionManager.instance != null && SceneTransitionManager.instance.playerPosition != null)
        {
            Vector3 pos = SceneTransitionManager.instance.playerPosition;
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                player.transform.position = pos;
        }
    }
}