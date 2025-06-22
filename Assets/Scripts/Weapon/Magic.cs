using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Magic : MonoBehaviour
{
    private Wand wand;
    private HashSet<IDamageable> hitTargets;
    
    [SerializeField] private GameObject magicEffect;
    [SerializeField] private float duration; // In seconds
    [SerializeField] private AudioClip sfx;
    
    public void Init(Wand _wand)
    {
        wand = _wand;
        hitTargets = new HashSet<IDamageable>();
        AudioManager.Instance.PlaySFX(sfx);
        Destroy(this.gameObject, duration);
        
        int magicCount = Random.Range(wand.MinMagicCount, wand.MaxMagicCount+1);

        for (int i = 0; i < magicCount; ++i)
        {
            Vector2 pos = Random.insideUnitCircle * wand.GetFinalStat("MagicRadius");
            GameObject effect = Instantiate(
                magicEffect, transform.position + (Vector3)pos, 
                Quaternion.identity, transform);
            
            float scale = Random.Range(wand.MinMagicScale, wand.MaxMagicScale);
            effect.transform.localScale *= scale;
            effect.transform.localScale *= wand.GetFinalStat("Size");
        }
    } 
    
    public void ResetEffect()
    {
        hitTargets.Clear();
        gameObject.SetActive(true);
        // 타이머 또는 외부에서 disable 처리
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (wand == null)
        {
            Debug.LogError("Weapon is null");
            return;
        }

        if (other.tag == "Player")
            return;
        
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable)
            && !hitTargets.Contains(damageable))
        {
            wand.OnHitDamageable(damageable);
            hitTargets.Add(damageable);
        }
    }
}