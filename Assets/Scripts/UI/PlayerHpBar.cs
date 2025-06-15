using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image image;
    private void Start()
    {
        player.CombatStatus.OnHpChanged += Refresh;
    }

    private void Refresh()
    {
        image.fillAmount = player.CombatStatus.HPRatio;
    }
}