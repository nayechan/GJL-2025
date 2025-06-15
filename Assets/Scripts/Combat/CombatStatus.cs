using System;
using UnityEngine;

[System.Serializable]
public class CombatStatus
{
    public event Action OnExpChanged, OnHpChanged;
    
    public long level = 1;

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
    
    public long MaxHp => (long)(Mathf.Pow(level, 1.5f) * 10 + 90);
    private long currentHp = 100;
    public long CurrentHp
    {
        get => currentHp;
        private set
        {
            if (currentHp != value)
            {
                currentHp = value;
                OnHpChanged?.Invoke();
            }
        }
    }

    [Header("Stats")]
    public long attackPower = 10;
    public long defense = 2;

    public bool IsDead => currentHp <= 0;

    /// <summary>
    /// 데미지를 계산하고 체력을 감소시킵니다. 방어력이 적용된 최종 데미지를 출력합니다.
    /// 방어력 적용 및 음수 방지 포함
    /// </summary>
    public long ApplyDamage(Damage damage)
    {
        long rawDamage = damage.Amount;
        long reducedDamage = Math.Max(0, rawDamage - defense);
        CurrentHp = Math.Max(0, currentHp - reducedDamage);

        return reducedDamage;
    }

    /// <summary>
    /// 체력을 회복합니다.
    /// </summary>
    public void Heal(long amount)
    {
        CurrentHp = Math.Min(MaxHp, currentHp + amount);
    }

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
        currentHp = MaxHp;
        OnExpChanged?.Invoke();
        OnHpChanged?.Invoke();
    }

    public void InitialCallback()
    {
        OnExpChanged?.Invoke();
        OnHpChanged?.Invoke();
    }

    /// <summary>
    /// 현재 체력을 퍼센트로 반환합니다.
    /// </summary>
    public float HPRatio => MaxHp > 0 ? (float)currentHp / MaxHp : 0f;

    public float ExpRatio => MaxExp > 0 ? (float)currentExp / MaxExp : 0f;
}