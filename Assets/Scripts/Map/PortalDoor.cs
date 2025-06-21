using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDoor : MonoBehaviour
{
    [SerializeField] private GameObject lockedDoor, unlockedDoor;
    private Window floorClearWindow;
    private bool isTriggered = false;

    private void Start()
    {
        floorClearWindow = WindowManager.Instance.GetWindow("FloorClearWindow");
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
        
        GameManager.Instance.PauseGame();
        floorClearWindow.Open();
    }
}
