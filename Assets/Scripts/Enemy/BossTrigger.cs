using System;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private Boss boss;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            boss?.OnPlayerEnterRegion(player, this);
        }
    }
}