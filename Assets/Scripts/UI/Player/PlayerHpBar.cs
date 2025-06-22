using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    private Player player;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>Player.Instance != null);
        player = Player.Instance;
        player.CombatStatus.OnHpChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        if (player == null) return;
        
        image.fillAmount = player.CombatStatus.GetHpRatio(player.GetMaxHp());
        text.text = $"{new string('♥', player.CombatStatus.life)}";
        text.text += $" {player.CombatStatus.CurrentHp:N0} / {player.GetMaxHp():N0}";
    }

    private void OnDestroy()
    {
        if(player != null)
            player.CombatStatus.OnHpChanged -= Refresh;
    }
}