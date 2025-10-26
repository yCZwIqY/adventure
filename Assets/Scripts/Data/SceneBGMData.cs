using UnityEngine;

[CreateAssetMenu(fileName = "SceneBGMData", menuName = "Audio/Scene BGM Data")]
public class SceneBGMData : ScriptableObject
{
    [System.Serializable]
    public struct SceneBGM
    {
        public string sceneName;
        public AudioClip bgmClip;
    }

    public SceneBGM[] sceneBGMs;
}