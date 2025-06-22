using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalKey : MonoBehaviour
{
    [SerializeField] private AudioClip gainKeySound;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GameObject portalDoor = GameObject.FindGameObjectWithTag("Door");
            portalDoor.GetComponent<PortalDoor>().Activate();
            
            AudioManager.Instance.PlaySFX(gainKeySound);
            
            Destroy(gameObject);
        }
    }
}
