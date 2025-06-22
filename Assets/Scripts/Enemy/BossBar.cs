using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    private Boss boss;
    [SerializeField] private GameObject bossBarContainer;
    [SerializeField] private Image fg;
    [SerializeField] private TMP_Text hpAmountTextField;
    
    public static BossBar Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Activate(Boss _boss)
    {
        bossBarContainer.SetActive(true);
        boss = _boss;
        boss.CombatStatus.OnHpChanged += Refresh;
        Refresh();
    }

    public void Deactivate()
    {
        boss.CombatStatus.OnHpChanged -= Refresh;
        boss = null;
        bossBarContainer.SetActive(false);
    }

    public void Refresh()
    {
        CombatStatus bossStatus = boss.CombatStatus;
        fg.fillAmount = bossStatus.GetHpRatio(boss.GetMaxHp());
        hpAmountTextField.text = $"{bossStatus.CurrentHp:N0} / {boss.GetMaxHp():N0}";
    }
}
