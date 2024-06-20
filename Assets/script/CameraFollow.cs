using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 플레이어 오브젝트
    public Transform player;
    // 카메라의 y축 오프셋
    public float yOffset = -2f;
    // 카메라가 이동할 수 있는 좌, 우 한계
    public float minX, maxX;

    void LateUpdate()
    {
        if (player != null)
        {
            // 카메라의 새로운 위치 계산
            float targetX = Mathf.Clamp(player.position.x, minX, maxX);
            float targetY = player.position.y + yOffset;

            // 카메라 위치 설정
            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
    }
}

