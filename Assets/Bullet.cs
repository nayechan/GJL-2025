using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IAttacker
{
    [SerializeField] private float bulletSpeed = 5.0f;
    private float attackDelay = 1.0f;
    private IAttacker source;
    private int damage;
    private Vector2 direction;

    private bool isCooldown = false;
    
    public CombatStatus CombatStatus => source.CombatStatus;

    public void Init(IAttacker _source, int _damage, Vector2 _direction)
    {
        source = _source;
        damage = _damage;
        direction = _direction.normalized;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 160.0f);
    }
    
    public void Attack(IDamageable damageable)
    {
        if(damageable.GetType() == typeof(Player))
            damageable.TakeDamage(new Damage(damage, source, DamageType.Physical, false));
    }

    private void Update()
    {
        transform.Translate(bulletSpeed * Time.deltaTime * direction, Space.World);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player) && !isCooldown)
        {
            Attack(player);
            isCooldown = true;
            StartCoroutine(ResetCooldown());
        }
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(attackDelay);
        isCooldown = false;
    }

}
