using System;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class CombatStatus
{
    public event Action OnHpChanged;
    
    public long level = 1;

    public long[] baseStats = new long[(int)BaseStatType.LENGTH];

    private long currentHp;
    public long CurrentHp
    {
        get => currentHp;
        set
        {
            if (currentHp != value)
            {
                currentHp = value;
                OnHpChanged?.Invoke();
            }
        }
    }

    public int baseMaxHp = 10;
    public long MaxHp => (baseStats[(int)BaseStatType.VIT] + 1) * baseMaxHp;

    public int life = 1;
    public int maxLife = 1;

    public bool IsDead => currentHp <= 0;

    /// <summary>
    /// 데미지를 계산하고 체력을 감소시킵니다. 방어력이 적용된 최종 데미지를 출력합니다.
    /// 방어력 적용 및 음수 방지 포함
    /// </summary>
    public long ApplyDamage(Damage damage)
    {
        long rawDamage = damage.Amount;
        
        CurrentHp = Math.Max(0, currentHp - rawDamage);

        return rawDamage;
    }

    /// <summary>
    /// 체력을 회복합니다.
    /// </summary>
    public void Heal(long amount)
    {
        CurrentHp = Math.Min(MaxHp, currentHp + amount);
    }

    public void CompleteHeal()
    {
        CurrentHp = MaxHp;
    }
    
    public void Init()
    {
        currentHp = MaxHp;
        OnHpChanged?.Invoke();
    }

    /// <summary>
    /// 현재 체력을 퍼센트로 반환합니다.
    /// </summary>
    public float HPRatio => MaxHp > 0 ? (float)currentHp / MaxHp : 0f;

    public void RefreshStat()
    {
        OnHpChanged?.Invoke();
    }
}