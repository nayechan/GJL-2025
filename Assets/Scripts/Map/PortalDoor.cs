using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDoor : MonoBehaviour
{
    [SerializeField] private GameObject lockedDoor, unlockedDoor;
    private bool isTriggered = false;

    private void Start()
    {
    }

    public void Activate()
    {
        lockedDoor.SetActive(false);
        unlockedDoor.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered || other.tag != "Player") return;
        
        isTriggered = true;
        
        StageManager.Instance.ClearFloor();
    }
}
