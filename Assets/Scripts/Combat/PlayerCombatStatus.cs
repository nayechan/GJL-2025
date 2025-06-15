using System;
using UnityEngine;

[System.Serializable]
public class PlayerCombatStatus : CombatStatus
{
    public event Action OnExpChanged;

    public long MaxExp => (long)(Mathf.Pow(level, 1.5f) * 20 + 20);
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
    
    public override long MaxHp => (long)(Mathf.Pow(level, 1.5f) * 10 + 90);

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
            ++level;
        }
        OnExpChanged?.Invoke();
        CurrentHp = MaxHp;
    }

    public float ExpRatio => MaxExp > 0 ? (float)currentExp / MaxExp : 0f;
    
    public override void InitialCallback()
    {
        base.InitialCallback();
        OnExpChanged?.Invoke();
    }

}