using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : MonoBehaviour
{
    private Player player;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text levelText, expText;
    
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>Player.Instance != null);
        player = Player.Instance;
        PlayerStatus playerStatus = player.PlayerStatus;
        playerStatus.OnExpChanged.AddListener(Refresh);
        Refresh();
    }

    private void Refresh()
    {
        if (player == null) return;
        
        PlayerStatus playerStatus = player.PlayerStatus;
        image.fillAmount = playerStatus.ExpRatio;
        levelText.text = $"Lv. {playerStatus.Level:N0}";
        expText.text = $"★ {playerStatus.CurrentExp:N0} / {playerStatus.MaxExp:N0}";
    }

    private void OnDestroy()
    {
        if (player == null) return;

        PlayerStatus playerStatus = player.PlayerStatus;
        playerStatus.OnExpChanged.RemoveListener(Refresh);
    }
}