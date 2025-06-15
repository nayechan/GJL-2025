using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected Player player;
    
    [field: SerializeField]
    public float Cooldown { get; protected set; }
    
    [field: SerializeField]
    public long BaseDamage { get; protected set; }
    
    protected float lastAttackTime;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    
    public abstract DamageType GetDamageType();

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
}
