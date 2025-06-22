using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    private Player player;
    [SerializeField] private GameObject content;

    IEnumerator Start()
    {
        yield return new WaitUntil(()=>Player.Instance != null);
        player = Player.Instance;
        player.OnGameOver.AddListener(ActivateGameOverPanel);
    }

    public void ActivateGameOverPanel()
    {
        content.SetActive(true);
    }

    public void ClickConfirmButton()
    {
        Destroy(player.gameObject);
        GameManager.Instance.EndGame();
    }

    private void OnDestroy()
    {
        if(player != null)
            player.OnGameOver.RemoveListener(ActivateGameOverPanel);
    }
}