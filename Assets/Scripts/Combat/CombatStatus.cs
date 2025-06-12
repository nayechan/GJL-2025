using System;
using UnityEngine;

[System.Serializable]
public class CombatStatus
{
    [Header("Health")]
    public long maxHP = 100;
    public long currentHP = 100;

    [Header("Stats")]
    public long attackPower = 10;
    public long defense = 2;

    public bool IsDead => currentHP <= 0;

    /// <summary>
    /// 데미지를 계산하고 체력을 감소시킵니다. 방어력이 적용된 최종 데미지를 출력합니다.
    /// 방어력 적용 및 음수 방지 포함
    /// </summary>
    public long ApplyDamage(Damage damage)
    {
        long rawDamage = damage.Amount;
        long reducedDamage = Math.Max(0, rawDamage - defense);
        currentHP = Math.Max(0, currentHP - reducedDamage);

        return reducedDamage;
    }

    /// <summary>
    /// 체력을 회복합니다.
    /// </summary>
    public void Heal(long amount)
    {
        currentHP = Math.Min(maxHP, currentHP + amount);
    }

    /// <summary>
    /// 현재 체력을 퍼센트로 반환합니다.
    /// </summary>
    public float HPRatio => maxHP > 0 ? (float)currentHP / maxHP : 0f;
}