using UnityEngine;

public enum GroundType { Grass, Wood, Stone }

public class Ground : MonoBehaviour
{
    public GroundType groundType = GroundType.Grass;
}