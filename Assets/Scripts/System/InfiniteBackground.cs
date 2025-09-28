using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    public Transform player;
    public List<GameObject> backgrounds;
    public List<float> parallaxFactors; // 각 레이어별 X축 패럴랙스 비율
    public float yFactor = 0.5f; // Y축 이동 비율 (0~1)

    private Vector3 lastPlayerPos;

    private void Start()
    {
        if (!player || backgrounds.Count == 0) return;
        lastPlayerPos = player.position;
    }

    private void LateUpdate()
    {
        if (!player || backgrounds.Count == 0) return;

        Vector3 deltaMovement = player.position - lastPlayerPos;

        for (int i = 0; i < backgrounds.Count; i++)
        {
            float factor = (i < parallaxFactors.Count) ? parallaxFactors[i] : 1f;

            backgrounds[i].transform.position += new Vector3(
                deltaMovement.x * factor,       // X축은 레이어별 factor 적용
                deltaMovement.y * factor * yFactor, // Y축은 절반만 적용
                0f
            );
        }

        lastPlayerPos = player.position;
    }
}