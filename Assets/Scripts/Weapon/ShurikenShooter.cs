using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add Shuriken Shooter", fileName = "New Shuriken Shooter")]
public class ShurikenShooter : Weapon
{
    [SerializeField] private GameObject shurikenInstancePrefab;
    [SerializeField] private AudioClip shurikenShotSound;
    
    [field: SerializeField] public int ShurikenCount { get; private set; } = 3;
    [field: SerializeField] public int PenetrationCount { get; private set; } = 0;

    protected override void Attack()
    {
        Vector2 baseDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition)
                                 - player.WeaponTransform.position);
        baseDirection.Normalize();

        int count = (int)GetFinalStat("Count");
        
        for (int current = 0; current < count; ++current)
        {
            Vector2 direction = Quaternion.Euler(0,0,(current - (count-1) / 2.0f) * 10.0f) * baseDirection;
        
            Shuriken shuriken = 
                Instantiate(shurikenInstancePrefab, player.WeaponTransform.position, Quaternion.identity)
                .GetComponent<Shuriken>();
            
            shuriken.Init(this, direction, (int)GetFinalStat("Penetration"));
            
            AudioManager.Instance.PlaySFX(shurikenShotSound);
        }
    }

    public override void OnUse(Player _player)
    {
        _player.EquipWeapon(this);
    }

    public override string GetWeaponType()
    {
        return "Shuriken";
    }

    public override float GetBaseStat(string statType)
    {
        switch (statType)
        {
            case "Count":
                return ShurikenCount;
            case "Penetration":
                return PenetrationCount;
            default:
                return base.GetBaseStat(statType);
        }
    }
}