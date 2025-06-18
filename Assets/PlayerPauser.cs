using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPauser : MonoBehaviour
{
    [SerializeField] private Player player;

    private void OnEnable()
    {
        player.enabled = false;
        Time.timeScale = 0.0f;
    }

    private void OnDisable()
    {
        player.enabled = true;
        Time.timeScale = 1.0f;
    }
}
