using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);
    
    [Header("카메라 한계값")]
    public Vector2 minPosition; // 최소 x, y
    public Vector2 maxPosition; // 최대 x, y


    private void FixedUpdate()
    {
        if (!player) return;

        Vector3 targetPos = player.position + offset;

        // 부드럽게 따라가기
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, smoothSpeed);

        // 픽셀 스냅으로 덜덜거림 방지
        smoothPos.x = Mathf.Round(smoothPos.x * 1000f) / 1000f;
        smoothPos.y = Mathf.Round(smoothPos.y * 1000f) / 1000f;

        // 카메라 이동 범위 제한
        smoothPos.x = Mathf.Clamp(smoothPos.x, minPosition.x, maxPosition.x);
        smoothPos.y = Mathf.Clamp(smoothPos.y, minPosition.y, maxPosition.y);

        transform.position = smoothPos;
    }
}