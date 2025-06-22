using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add Sword", fileName = "New Sword")]
public class Sword : Weapon
{
    [SerializeField] private GameObject swordEffectPrefab;

    protected override void Attack()
    {
        AttackEffect attackEffect = 
            Instantiate(swordEffectPrefab, player.WeaponTransform).GetComponent<AttackEffect>();

        attackEffect.transform.localScale *= GetFinalStat("Size");
        attackEffect.Init(this);
    }

    public override void OnUse(Player _player)
    {
        _player.EquipWeapon(this);
    }

    public override string GetWeaponType()
    {
        return "Sword";
    }
}