using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    private Weapon weapon;
    private HashSet<IDamageable> hitTargets;
    
    [SerializeField] private float duration; // In seconds
    [SerializeField] private AudioClip sfx;
    
    public void Init(Weapon _weapon)
    {
        weapon = _weapon;
        hitTargets = new HashSet<IDamageable>();
        AudioManager.Instance.PlaySFX(sfx);
        Destroy(this.gameObject, duration);
    }
    
    public void ResetEffect()
    {
        hitTargets.Clear();
        gameObject.SetActive(true);
        // 타이머 또는 외부에서 disable 처리
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (weapon == null)
        {
            Debug.LogError("Weapon is null");
            return;
        }

        if (other.tag == "Player")
            return;
        
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable)
            && !hitTargets.Contains(damageable))
        {
            weapon.OnHitDamageable(damageable);
            hitTargets.Add(damageable);
        }
    }
}