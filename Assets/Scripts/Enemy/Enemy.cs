using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour, IAttacker, IDamageable
{
    [SerializeField] private CombatStatus combatStatus;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float knockBackIntensity = 5.0f; // 넉백 강도
    [SerializeField] private float knockbackDuration = 0.3f; // 넉백 지속 시간
    [SerializeField] private float dragCoefficient = 5f; // 자연스러운 감속을 위한 드래그
    
    [SerializeField] private Transform followTarget;

    private bool isCooldown = false;
    private bool isKnockedBack = false; // 넉백 상태 플래그
    private Vector2 direction;
    private Rigidbody2D rb;
    private Coroutine knockbackCoroutine;

    public CombatStatus CombatStatus => combatStatus;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        followTarget = GameObject.FindGameObjectWithTag("Player").transform;
        combatStatus.Init(level => 5 * (level + 1));
    }

    private void Update()
    {
        CalculateKnockback();
    }

    public void CalculateKnockback()
    {
        if (followTarget == null || isKnockedBack) return;

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
        
        // 넉백 처리
        ApplyKnockback(-direction);

        Debug.Log($"Enemy took {finalDamage} damage. HP: {combatStatus.CurrentHp}");

        if (combatStatus.IsDead)
        {
            if (damage.Source.GetType() == typeof(Player))
            {
                Player player = (Player)damage.Source;
                player.PlayerStatus.GainExp(combatStatus.level * 5);
            }
            Die();
        }
    }

    private void ApplyKnockback(Vector2 knockbackDirection)
    {
        // 기존 넉백 코루틴이 있다면 중단
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }
        
        // 넉백 적용
        rb.AddForce(knockbackDirection * knockBackIntensity, ForceMode2D.Impulse);
        
        // 넉백 상태 관리 코루틴 시작
        knockbackCoroutine = StartCoroutine(KnockbackCoroutine());
    }

    private IEnumerator KnockbackCoroutine()
    {
        isKnockedBack = true;
        
        // 넉백 지속 시간 대기
        yield return new WaitForSeconds(knockbackDuration);
        
        isKnockedBack = false;
        knockbackCoroutine = null;
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