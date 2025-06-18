using System;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject content;

    private void Start()
    {
        player.OnGameOver.AddListener(ActivateGameOverPanel);
    }

    public void ActivateGameOverPanel()
    {
        content.SetActive(true);
    }
}