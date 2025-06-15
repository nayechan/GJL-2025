using System;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    private Weapon weapon;
    [SerializeField] private float duration; // In seconds
    public void Init(Weapon _weapon)
    {
        weapon = _weapon;
        Destroy(this.gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            weapon.OnHitDamageable(damageable);
        }
    }
}