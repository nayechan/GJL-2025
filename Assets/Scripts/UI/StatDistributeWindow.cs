using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;


public class StatDistributeWindow : Window
{
    [SerializeField] private Button[] statButtons;
    [SerializeField] private TMP_Text[] statTextFields;
    [SerializeField] private TMP_Text apTextField;

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
        Player player = Player.Instance;
        if (player.PlayerStatus.Ap > 0)
        {
            ++player.CombatStatus.baseStats[(int)statType];
            --player.PlayerStatus.Ap;
        }
        Refresh();
    }

    private void Refresh()
    {
        Player player = Player.Instance;
        long ap = player.PlayerStatus.Ap;
        
        for (int i = 0; i < (int)BaseStatType.LENGTH; i++)
        {
            statTextFields[i].text = $"{player.CombatStatus.baseStats[i]:N0}";
            statButtons[i].interactable = (ap > 0);
        }

        // Localization key: "APText" in "Localization Table"
        // Example value in table: "{0} Points Left"
        LocalizedString apTextString = new LocalizedString("Localization Table", "APText");
        apTextString.Arguments = new object[] { ap.ToString("N0", CultureInfo.InvariantCulture) };

        apTextString.StringChanged += (localizedText) =>
        {
            apTextField.text = localizedText;
        };
    }
}
