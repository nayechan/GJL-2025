using System;
using UnityEngine;

public class PlayerStatus
{
    private CombatStatus combatStatus;
    public event Action OnExpChanged;
    
    private long currentExp = 0;
    public long CurrentExp
    {
        get => currentExp;
        private set
        {
            currentExp = value;
            OnExpChanged?.Invoke();
        }
    }

    public long MaxExp => (long)(Mathf.Pow(combatStatus.level, 1.5f) * 20 + 20);
    
    /// <summary>
    /// 경험치를 얻고, currentExp > MaxExp일때 레벨업합니다
    /// </summary>
    public void GainExp(long amount)
    {
        CurrentExp += amount;
        if (CurrentExp >= MaxExp)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        while (currentExp >= MaxExp)
        {
            currentExp -= MaxExp;
            ++combatStatus.level;
        }
        OnExpChanged?.Invoke();
        combatStatus.CompleteHeal();
    }
    
    public long Level => combatStatus.level;

    public float ExpRatio => MaxExp > 0 ? (float)currentExp / MaxExp : 0f;
    
    public PlayerStatus(CombatStatus _combatStatus)
    {
        combatStatus = _combatStatus;
        OnExpChanged?.Invoke();
    }

}