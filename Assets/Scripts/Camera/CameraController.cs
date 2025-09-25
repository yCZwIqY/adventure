using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    private void LateUpdate()
    {
        if (!player) return;

        Vector3 targetPos = player.position + offset;

        // 부드럽게 따라가기
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, smoothSpeed);

        // 픽셀 스냅으로 덜덜거림 방지
        smoothPos.x = Mathf.Round(smoothPos.x * 100f) / 100f;
        smoothPos.y = Mathf.Round(smoothPos.y * 100f) / 100f;
        smoothPos.z = -10f; // Z 고정

        transform.position = smoothPos;
    }
}
