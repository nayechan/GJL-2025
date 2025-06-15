using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IAttacker, IDamageable
{
    [SerializeField] private CombatStatus combatStatus;
    [SerializeField] private float attackDelay = 0.5f;

    private bool isCooldown = false;

    public CombatStatus CombatStatus => combatStatus;

    private void Start()
    {
        combatStatus.InitialCallback();
    }

    public Damage RollAttack()
    {
        // 간단하게 공격력 그대로 데미지 생성, 확장 가능
        return new Damage(combatStatus.attackPower, this, DamageType.Physical);
    }

    // IAttacker 구현
    public void Attack(IDamageable target)
    {
        Damage damage = RollAttack();

        Debug.Log($"Enemy dealt {damage.Amount} damage. HP: {combatStatus.CurrentHp}");
        
        target.TakeDamage(damage);
    }

    // IDamageable 구현
    public void TakeDamage(Damage damage)
    {
        long finalDamage = combatStatus.ApplyDamage(damage);

        Debug.Log($"Enemy took {finalDamage} damage. HP: {combatStatus.CurrentHp}");

        if (combatStatus.IsDead)
            Die();
    }

    private void Die()
    {
        Debug.Log("Enemy Died");
        // 사망 처리 (사망 애니메이션, 제거, 보상 등)
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && !isCooldown)
        {
            Player player = other.GetComponent<Player>();
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