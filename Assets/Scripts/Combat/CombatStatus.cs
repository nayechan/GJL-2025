using System;
using UnityEngine;

[System.Serializable]
public class CombatStatus
{
    public event Action OnHpChanged;
    
    public long level = 1;

    public long attackPower = 10;
    public long defense = 2;

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

    public Func<long, long> GetMaxHp;

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
        long reducedDamage = Math.Max(0, rawDamage - defense);
        CurrentHp = Math.Max(0, currentHp - reducedDamage);

        return reducedDamage;
    }

    /// <summary>
    /// 체력을 회복합니다.
    /// </summary>
    public void Heal(long amount)
    {
        CurrentHp = Math.Min(GetMaxHp(level), currentHp + amount);
    }

    public void CompleteHeal()
    {
        CurrentHp = GetMaxHp(level);
    }
    
    public virtual void Init(Func<long, long> _MaxHp)
    {
        GetMaxHp = _MaxHp;
        currentHp = GetMaxHp(level);
        OnHpChanged?.Invoke();
    }

    /// <summary>
    /// 현재 체력을 퍼센트로 반환합니다.
    /// </summary>
    public float HPRatio => GetMaxHp(level) > 0 ? (float)currentHp / GetMaxHp(level) : 0f;
}