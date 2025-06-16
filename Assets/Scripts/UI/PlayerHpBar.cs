using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    private void Start()
    {
        player.CombatStatus.OnHpChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        image.fillAmount = player.CombatStatus.HPRatio;
        text.text = $"{new string('♥', player.CombatStatus.life)} {player.CombatStatus.HPRatio:P1}";
    }
}