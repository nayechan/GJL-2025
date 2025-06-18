using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StatDistributer : MonoBehaviour
{
    [SerializeField] private Button[] statButtons;
    [SerializeField] private TMP_Text[] statTextFields;
    [SerializeField] private TMP_Text apTextField;

    [SerializeField] private Player player;

    private void OnEnable()
    {
        Refresh();
    }

    private void Awake()
    {
        for (BaseStatType statType = 0; statType < BaseStatType.LENGTH; statType++)
        {
            RegisterStatButton(statType);
        }
    }

    private void RegisterStatButton(BaseStatType statType)
    {
        statButtons[(int)statType].onClick.AddListener(() => AddStat(statType));
    }

    private void AddStat(BaseStatType statType)
    {
        if (player.PlayerStatus.Ap > 0)
        {
            ++player.CombatStatus.baseStats[(int)statType];
            --player.PlayerStatus.Ap;
        }
        Refresh();
    }

    private void Refresh()
    {
        long ap = player.PlayerStatus.Ap;
        
        for (int i = 0; i < (int)BaseStatType.LENGTH; i++)
        {
            statTextFields[i].text = $"{player.CombatStatus.baseStats[i]:N0}";
            statButtons[i].interactable = (ap > 0);
        }

        apTextField.text = $"{player.PlayerStatus.Ap:N0} Points Left";
    }
}
