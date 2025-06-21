using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Add Shuriken Shooter", fileName = "New Shuriken Shooter")]
public class ShurikenShooter : Weapon
{
    [SerializeField] private GameObject shurikenInstancePrefab;

    protected override void Attack()
    {
        Vector2 baseDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition)
                                 - player.WeaponTransform.position);
        baseDirection.Normalize();


        int count = 3;
        for (int current = 0; current < 3; ++current)
        {
            Vector2 direction = Quaternion.Euler(0,0,(current - (count-1) / 2.0f) * 10.0f) * baseDirection;
        
            Shuriken shuriken = 
                Instantiate(shurikenInstancePrefab, player.WeaponTransform.position, Quaternion.identity)
                .GetComponent<Shuriken>();
            
            shuriken.Init(this, direction);
        }
    }

    public override void OnUse(Player _player)
    {
        _player.EquipWeapon(this);
    }
}