using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class CombatStatus
{
    public event Action OnHpChanged;
    
    public long level = 1;

    public long[] baseStats = new long[(int)BaseStatType.LENGTH];

    private float currentHp;
    public float CurrentHp
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

    public int life = 1;
    public int maxLife = 3;

    public bool IsDead => currentHp <= 0;
    
    public void Init()
    {
        RefreshStat();
    }

    public void Init(int _level, bool isOverflown = false)
    {
        level = _level;
        
        baseStats[(int)BaseStatType.STR] *= AdjustSTR(_level, baseStats[(int)BaseStatType.STR]);
        baseStats[(int)BaseStatType.VIT] *= AdjustVIT(_level, baseStats[(int)BaseStatType.VIT]);
        
        if(isOverflown)
            baseStats[(int)BaseStatType.AGI] += 10;

        Init();
    }

    public long AdjustSTR(int _level, long value)
    {
        return Mathf.RoundToInt(0.5f * Mathf.Pow(_level, 1.4f) * value);
    }

    public long AdjustVIT(int _level, long value)
    {
        return Mathf.RoundToInt(0.5f * Mathf.Pow(_level, 1.5f) * value);
    }

    public float CalculateFinalStat(BaseStatType _baseStatType, List<IStatModifier> _modifiers = null)
    {
        float result = baseStats[(int)_baseStatType];

        if (_modifiers == null)
            return result;
        
        foreach (IStatModifier modifier in _modifiers)
        {
            if (modifier is BaseStatModifier)
            {
                BaseStatModifier baseStatModifier = (BaseStatModifier)modifier;
                if(baseStatModifier.StatType == _baseStatType)
                    result += baseStatModifier.Amount;
            }
        }

        return result;
    }

    /// <summary>
    /// 현재 체력을 퍼센트로 반환합니다.
    /// </summary>
    public float GetHpRatio(long maxHp)
    {
        return maxHp > 0 ? (float)currentHp / maxHp : 0f;
    }

    public void RefreshStat()
    {
        OnHpChanged?.Invoke();
    }

    public long ApplyDamage(Damage _damage)
    {
        CurrentHp -= _damage.Amount;
        if(CurrentHp < 0) currentHp = 0;
        return _damage.Amount;
    }
}