using System;
using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    [SerializeField] private GameObject swordEffectPrefab;

    protected override void Attack()
    {
        AttackEffect attackEffect = 
            Instantiate(swordEffectPrefab, player.AttackPos).GetComponent<AttackEffect>();
        
        attackEffect.Init(this);
    }

    public override DamageType GetDamageType() => DamageType.Physical;
}