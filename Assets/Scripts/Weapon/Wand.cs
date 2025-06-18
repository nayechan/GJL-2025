using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Add Wand", fileName = "New Wand")]
public class Wand : Weapon
{
    [SerializeField] private GameObject magicPrefab;
    [field: SerializeField] public int MinMagicCount { get; private set; } = 5;
    [field: SerializeField] public int MaxMagicCount { get; private set; } = 20;
    [field: SerializeField] public float MinMagicScale { get; private set; } = 2.0f;
    [field: SerializeField] public float MaxMagicScale { get; private set; } = 4.0f;
    [field: SerializeField] public float MagicRadius { get; private set; } = 10.0f;

    protected override void Attack()
    {
        Magic magic = Instantiate(magicPrefab, player.WeaponTransform.position, Quaternion.identity)
            .GetComponent<Magic>();
        
        magic.Init(this);
    }

    public override void OnUse(Player _player)
    {
        _player.EquipWeapon(this);
    }
}