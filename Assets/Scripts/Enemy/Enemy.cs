using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour, IAttacker, IDamageable
{
    [SerializeField] private CombatStatus combatStatus;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float moveSpeed = 3f;
    
    [SerializeField] private Transform followTarget;

    private bool isCooldown = false;
    private bool isParalyzed = false;
    private bool isKnockedBack = false; // 넉백 상태 플래그
    private Vector2 direction;
    private Rigidbody2D rb;
    private Coroutine knockbackCoroutine, paralyzeCoroutine;

    public CombatStatus CombatStatus => combatStatus;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        followTarget = GameObject.FindGameObjectWithTag("Player").transform;
        combatStatus.Init();
    }

    private void Update()
    {
        if (followTarget == null || isKnockedBack || isParalyzed) return;

        // 방향 벡터 계산
        direction = (followTarget.position - transform.position).normalized;
        
        // velocity를 사용한 부드러운 이동
        Vector2 targetVelocity = direction * moveSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, Time.deltaTime * 8f);
        
        // 방향에 따라 스케일 조절 (X축만)
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(-direction.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
    public Damage RollAttack()
    {
        // 간단하게 공격력 그대로 데미지 생성, 확장 가능
        return new Damage(combatStatus.baseStats[(int)BaseStatType.STR], this, DamageType.Physical);
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
        
        if(damage.KnockbackDuration > 0)
            ApplyKnockback(-direction, damage.KnockbackDuration, damage.KnockbackIntensity);
        
        if(damage.ParalyzeDuration > 0)
            ApplyParalyze(damage.ParalyzeDuration);
        
        DamageTextManager.Instance.ShowDamage(finalDamage, Color.white, transform, 1f);

        Debug.Log($"Enemy took {finalDamage} damage. HP: {combatStatus.CurrentHp}");

        if (combatStatus.IsDead)
        {
            if (damage.Source.GetType() == typeof(Player))
            {
                Player player = (Player)damage.Source;
                player.PlayerStatus.GainExp(combatStatus.level * 5);
                player.PlayerStatus.GainGold(combatStatus.level + 4);
            }
            Die();
        }
    }

    private void ApplyKnockback(Vector2 knockbackDirection, float knockbackDuration, float knockbackIntensity)
    {
        Debug.Log(knockbackIntensity);
        Debug.Log(knockbackDirection);
        
        // 기존 넉백 코루틴이 있다면 중단
        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        // 넉백 상태 관리 코루틴 시작
        knockbackCoroutine = StartCoroutine(
            KnockbackCoroutine(knockbackDirection, knockbackDuration, knockbackIntensity));
    }

    private void ApplyParalyze(float paralyzeDuration)
    {
        if(paralyzeCoroutine != null)
            StopCoroutine(paralyzeCoroutine);
        
        rb.velocity = Vector2.zero;
        
        paralyzeCoroutine = StartCoroutine(ParalyzeCoroutine(paralyzeDuration));
    }

    private IEnumerator KnockbackCoroutine(
        Vector2 knockbackDirection, float knockbackDuration, float knockbackIntensity)
    {
        isKnockedBack = true;
        
        // 넉백 지속 시간 대기
        float time = knockbackDuration;

        while (time > 0f)
        {
            float effectiveKnockbackIntensity = knockbackIntensity * (time / knockbackDuration);
            // 넉백 적용
            transform.Translate(effectiveKnockbackIntensity * Time.deltaTime * knockbackDirection);
            
            time -= Time.deltaTime;
            yield return null;
        }
        
        isKnockedBack = false;
        knockbackCoroutine = null;
    }

    private IEnumerator ParalyzeCoroutine(float paralyzeDuration)
    {
        isParalyzed = true;
        rb.isKinematic = true;

        yield return new WaitForSeconds(paralyzeDuration);
        
        isParalyzed = false;
        rb.isKinematic = false;
        paralyzeCoroutine = null;
    }

    private void Die()
    {
        Debug.Log("Enemy Died");
        // 사망 처리 (사망 애니메이션, 제거, 보상 등)
        Destroy(gameObject);
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(attackDelay);
        isCooldown = false;
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
}