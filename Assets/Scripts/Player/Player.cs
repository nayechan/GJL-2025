using UnityEngine;

public class Player : MonoBehaviour, IAttacker, IDamageable
{
    [SerializeField] private CombatStatus combatStatus;

    public CombatStatus CombatStatus => combatStatus;

    public Damage RollAttack()
    {
        return new Damage(combatStatus.attackPower, this, DamageType.Physical);
    }

    // IAttacker 구현
    public void Attack(IDamageable target)
    {
        Damage damage = RollAttack();

        Debug.Log($"Player dealt {damage.Amount} damage. HP: {combatStatus.currentHP}");
        
        target.TakeDamage(damage);
    }

    // IDamageable 구현
    public void TakeDamage(Damage damage)
    {
        long finalDamage = combatStatus.ApplyDamage(damage);

        Debug.Log($"Player took {finalDamage} damage. HP: {combatStatus.currentHP}");

        if (combatStatus.IsDead)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player Died");
        // 사망 처리
    }
}