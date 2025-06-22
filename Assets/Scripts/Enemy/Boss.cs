using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class Boss : IEnemy, IAttacker, IDamageable
{
    [SerializeField] private CombatStatus combatStatus;
    [SerializeField] private GameObject keyPrefab, bulletPrefab;
    [SerializeField] private float attackDelay = 0.5f;

    [SerializeField] private AudioClip onHitSound;

    private bool isCooldown = false;
    private bool isParalyzed = false;
    private bool isKnockedBack = false; // 넉백 상태 플래그
    private bool entered = false;
    
    private Vector2 direction;
    private Rigidbody2D rb;
    private Coroutine knockbackCoroutine, paralyzeCoroutine;
    private BossTrigger bossTrigger;

    public CombatStatus CombatStatus => combatStatus;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        combatStatus.Init(CalculateLevel());
        combatStatus.CurrentHp = GetMaxHp();
    }

    private void Update()
    {
        Player player = Player.Instance;

        if (player == null || player.CombatStatus.IsDead)
            return;
        
        if (player.transform.position.x > transform.position.x)
        {
            if(transform.position.x < 32)
                transform.Translate(Time.deltaTime * Vector3.right);
            transform.localScale = new Vector3(-2, 2, 1);
        }
        else
        {
            if(transform.position.x > 4)
                transform.Translate(Time.deltaTime * Vector3.left);
            transform.localScale = new Vector3(2, 2, 1);
        }
    }

    public long GetMaxHp()
    {
        float vit = combatStatus.CalculateFinalStat(BaseStatType.VIT);
        
        return Mathf.RoundToInt(5 * vit + 5);
    }

    public int CalculateLevel()
    {
        int floor = StageManager.Instance.CurrentFloor;
        return Mathf.RoundToInt(floor * 1.2f);
    }

    public Damage RollAttack()
    {
        // 간단하게 공격력 그대로 데미지 생성, 확장 가능
        return new Damage(Mathf.RoundToInt(combatStatus.CalculateFinalStat(BaseStatType.STR)), 
            this, DamageType.Physical, false);
    }

    // IAttacker 구현
    public void Attack(IDamageable target)
    {
        if (CombatStatus.IsDead)
            return;
        
        Damage damage = RollAttack();

        Debug.Log($"Enemy dealt {damage.Amount} damage. HP: {combatStatus.CurrentHp}");
        
        target.TakeDamage(damage);
    }

    // IDamageable 구현
    public void TakeDamage(Damage damage)
    {
        if (damage.Source.GetType() != typeof(Player))
            return;
        
        long finalDamage = combatStatus.ApplyDamage(damage);
        
        if(damage.KnockbackDuration > 0)
            ApplyKnockback(-direction, damage.KnockbackDuration, damage.KnockbackIntensity);
        
        if(damage.ParalyzeDuration > 0)
            ApplyParalyze(damage.ParalyzeDuration);
        
        DamageTextManager.Instance.ShowDamage(finalDamage, damage.IsCritical, Color.white, transform, 4f);

        Debug.Log($"Enemy took {finalDamage} damage. HP: {combatStatus.CurrentHp}");
        //AudioManager.Instance.PlaySFX(onHitSound);

        if (combatStatus.IsDead)
        {
            if (damage.Source.GetType() == typeof(Player))
            {
                Player player = (Player)damage.Source;
                player.PlayerStatus.GainExp(3 + combatStatus.level * 4);
                player.PlayerStatus.GainGold(2 + combatStatus.level * 3);
            }
            Die();
        }
    }

    private void ApplyKnockback(Vector2 knockbackDirection, float knockbackDuration, float knockbackIntensity)
    {
        
    }

    private void ApplyParalyze(float paralyzeDuration)
    {
        if(paralyzeCoroutine != null)
            StopCoroutine(paralyzeCoroutine);
        
        rb.velocity = Vector2.zero;
        
        paralyzeCoroutine = StartCoroutine(ParalyzeCoroutine(paralyzeDuration / 2.0f));
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

    protected override void Die()
    {
        BossBar.Instance.Deactivate();
        Instantiate(keyPrefab, transform.position, Quaternion.identity);
        base.Die();
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

    public void OnPlayerEnterRegion(Player _player, BossTrigger _bossTrigger)
    {
        if (entered) return;

        entered = true;
        bossTrigger = _bossTrigger;
        BossBar.Instance.Activate(this);
        StartCoroutine(CombatProgress(_player));
    }

    IEnumerator CombatProgress(Player _player)
    {
        float shootDelay = 2.0f;
            
        while (true)
        {

            Vector3 baseDir = (_player.transform.position - 1.0f * Vector3.up - transform.position).normalized;

            for (int i = 0; i < 6; ++i)
            {
                Bullet bullet = Instantiate(
                        bulletPrefab, transform.position + 1.5f * Vector3.up, Quaternion.identity)
                    .GetComponent<Bullet>();
                bullet.Init(this, 
                    Mathf.RoundToInt(0.5f * combatStatus.CalculateFinalStat(BaseStatType.STR)),
                    Quaternion.Euler(0,0,60.0f * i) * baseDir);
            }
            
            yield return new WaitForSeconds(shootDelay);
        }
    }
}