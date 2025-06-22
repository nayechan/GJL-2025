using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat.Modifier;
using UnityEngine;

public abstract class Weapon : ScriptableObject, IEquippable
{
    protected Player player;
    protected GameObject weaponGameObj;
    
    [field: SerializeField]
    public Sprite Sprite { get; protected set; }
    
    [field: SerializeField]
    public GameObject BaseWeaponPrefab { get; protected set; }
    
    [field: SerializeField]
    public float Cooldown { get; protected set; }
    
    [field: SerializeField]
    public long BaseDamage { get; protected set; }

    [field: SerializeField] public float BaseCriticalRate { get; protected set; } = 0.1f;
    [field: SerializeField] public float BaseCriticalDamage { get; protected set; } = 0.5f;
    
    [field: SerializeField] public float KnockbackIntensity { get; protected set; } = 0.0f;
    [field: SerializeField] public float KnockbackDuration { get; protected set; } = 0.0f;
    [field: SerializeField] public float ParalyzeDuration { get; protected set; } = 0.0f;

    [field: SerializeField] public float ScaleModifier { get; protected set; } = 1.0f;
    [field: SerializeField] public float BaseDrain { get; protected set; } = 0.0f;
    [field: SerializeField] public DamageType DamageType { get; protected set; }
    
    protected float lastAttackTime;
    protected Vector3 originalScale;
    
    public Dictionary<string, float> FinalStat { get; private set; }

    public void Init(Player _player, GameObject _weaponGameObj)
    {
        player = _player;
        weaponGameObj = _weaponGameObj;
        
        FinalStat = new Dictionary<string, float>();
        
        UpdateFinalStat(_player.StatModifiers);
        
        originalScale = _weaponGameObj.transform.localScale;
        _weaponGameObj.transform.localScale *= GetFinalStat("Size");
    }

    public virtual bool TryAttack()
    {
        if (Time.time - lastAttackTime >= GetFinalStat("Cooldown"))
        {
            Attack();
            lastAttackTime = Time.time;
            return true;
        }

        return false;
    }
    
    public virtual void OnHitDamageable(IDamageable damageable)
    {
        Damage damage = player.RollAttack(this);
        if(damage != null)
            damageable.TakeDamage(damage);

        float drain = GetFinalStat("Drain");
        if (drain > 0)
            player.CombatStatus.CurrentHp += drain * damage.Amount;
    }

    protected abstract void Attack();
    public int Count => -1;
    public abstract void OnUse(Player player);

    public abstract string GetWeaponType();

    public void UpdateFinalStat(List<IStatModifier> modifiers)
    {
        FinalStat.Clear();
        
        foreach (IStatModifier statModifier in modifiers)
        {
            if (statModifier is WeaponStatModifier)
            {
                WeaponStatModifier weaponStatModifier = (WeaponStatModifier)statModifier;

                if (weaponStatModifier.WeaponType != GetWeaponType())
                    continue;
                
                string statType = weaponStatModifier.ModifierType;

                if (!FinalStat.ContainsKey(statType))
                    FinalStat[statType] = GetBaseStat(statType);
                    
                if(statType != "Cooldown")
                    FinalStat[statType] += weaponStatModifier.Amount;
                else
                {
                    FinalStat[statType] -= weaponStatModifier.Amount;
                    if(FinalStat[statType] < 0.5f)
                        FinalStat[statType] = 0.5f;
                }
            }
        }
    }

    public float GetFinalStat(string statType)
    {
        if(!FinalStat.ContainsKey(statType))
        {
            FinalStat[statType] = GetBaseStat(statType);
        }
        return FinalStat[statType];
    }
    
    public virtual float GetBaseStat(string statType)
    {
        switch (statType)
        {
            case "ATK":
                return BaseDamage;
            case "CriticalRate":
                return BaseCriticalRate;
            case "CriticalDamage":
                return BaseCriticalDamage;
            case "Size":
                return ScaleModifier;
            case "Cooldown":
                return Cooldown;
            case "Drain":
                return BaseDrain;
            default:
                return 0;
        }
    }

    public virtual void RefreshWeapon(List<IStatModifier> modifiers)
    {
        UpdateFinalStat(modifiers);
        weaponGameObj.transform.localScale = originalScale * GetFinalStat("Size");
    }
}
