using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;  // 플레이어 Transform
    [SerializeField] private float smoothSpeed = 5f;  // 부드러운 이동 속도
    [SerializeField] public float offsetY = 2.0f; // 카메라 Y 오프셋
    [SerializeField] public float fixedZ = -10.0f;

    private IEnumerator Start()
    {
        yield return new WaitUntil(()=>Player.Instance != null);
        player = Player.Instance.transform;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(player.position.x, player.position.y + offsetY, fixedZ);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}