using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker
{
    public CombatStatus CombatStatus { get; }
    public void Attack(IDamageable damageable);
}
