using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // �÷��̾� ������Ʈ
    public Transform player;
    // ī�޶��� y�� ������
    public float yOffset = -2f;
    // ī�޶� �̵��� �� �ִ� ��, �� �Ѱ�
    public float minX, maxX;

    void LateUpdate()
    {
        if (player != null)
        {
            // ī�޶��� ���ο� ��ġ ���
            float targetX = Mathf.Clamp(player.position.x, minX, maxX);
            float targetY = player.position.y + yOffset;

            // ī�޶� ��ġ ����
            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
    }
}

