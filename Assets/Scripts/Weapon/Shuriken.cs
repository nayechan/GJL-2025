using System;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    private ShurikenShooter shurikenShooter;
    private Vector3 direction;

    [SerializeField] private int penetrationCount = 3;
    
    [SerializeField] private float duration; // In seconds
    [SerializeField] private float speed = 20, rotateSpeed = 360;
    
    public void Init(ShurikenShooter _shurikenShooter, Vector2 _direction)
    {
        shurikenShooter = _shurikenShooter;
        direction = _direction;
        Destroy(this.gameObject, duration);
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * speed * direction, Space.World);
        transform.Rotate(Time.deltaTime * rotateSpeed * Vector3.forward);
    }

    public void ResetEffect()
    {
        gameObject.SetActive(true);
        // 타이머 또는 외부에서 disable 처리
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (shurikenShooter == null)
        {
            Debug.LogError("Weapon is null");
            return;
        }
        
        if (other.tag == "Player")
            return;
        
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            shurikenShooter.OnHitDamageable(damageable);
            --penetrationCount;
        }
        
        if(penetrationCount < 0)
            Destroy(gameObject);
    }
}