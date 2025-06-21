using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalKey : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GameObject portalDoor = GameObject.FindGameObjectWithTag("Door");
            portalDoor.GetComponent<PortalDoor>().Activate();
            
            Destroy(gameObject);
        }
    }
}
