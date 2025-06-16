using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : ScriptableObject, IEquippable
{
    protected Player player;
    
    [field: SerializeField]
    public Sprite Sprite { get; protected set; }
    
    [field: SerializeField]
    public GameObject BaseWeaponPrefab { get; protected set; }
    
    [field: SerializeField]
    public float Cooldown { get; protected set; }
    
    [field: SerializeField]
    public long BaseDamage { get; protected set; }

    [field: SerializeField] public float ScaleModifier { get; protected set; } = 1.0f;
    
    protected float lastAttackTime;
    
    public abstract DamageType GetDamageType();

    public void Init(Player _player)
    {
        player = _player;
    }

    public virtual bool TryAttack()
    {
        if (Time.time - lastAttackTime >= Cooldown)
        {
            Attack();
            lastAttackTime = Time.time;
            return true;
        }

        return false;
    }
    
    public virtual void OnHitDamageable(IDamageable damageable)
    {
        damageable.TakeDamage(player.RollAttack(this));
    }

    protected abstract void Attack();
    public int Count => -1;
    public abstract void OnUse(Player player);
}
