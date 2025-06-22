using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, IAttacker, IDamageable, IMovable
{
    [FormerlySerializedAs("moveSpeed")] [SerializeField]
    private float baseMoveSpeed = 5f;

    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private int maxJumpCount = 2;
    
    [SerializeField] private float coyoteTime = 0.05f;
    private float lastGroundedTime;
    private bool wasGrounded;
    private bool jumpRequested = false;


    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance = 0.1f, wallCheckHeight = 1.5f;
    [SerializeField] private float wallCheckYOffset = 0.05f;
    [SerializeField] private Transform wallCheckPoint;

    [SerializeField] private Weapon currentWeapon;
    private GameObject weaponInstance;

    [SerializeField] private int jumpCount = 0;

    [SerializeField] private CombatStatus combatStatus;
    [SerializeField] private PlayerStatus playerStatus;
    
    [SerializeField] private float invulnerabilityTime = 3.0f;

    [SerializeField] public bool isGrounded = true;


    private Rigidbody2D playerRigidbody;
    private Animator animator;
    private bool isInvulnerable = false;
    
    private Inventory inventory;
    public List<IStatModifier> StatModifiers { get; private set; }
    private Dictionary<string, float> extraStats;

    [field: SerializeField] public Transform WeaponTransform { get; private set; }

    public CombatStatus CombatStatus => combatStatus;
    public PlayerStatus PlayerStatus => playerStatus;

    public UnityEvent OnGameOver { get; private set; }
    
    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(this);
        
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        OnGameOver = new UnityEvent();
        StatModifiers = new List<IStatModifier>();
        extraStats = new Dictionary<string, float>();
        
        combatStatus.Init();
        playerStatus.Init(this);

        combatStatus.CurrentHp = GetMaxHp();

        StartCoroutine(RegenerateHp());
    }

    private void Start()
    {
        GameManager.Instance.onPause.AddListener(() => { enabled = false;});
        GameManager.Instance.onResume.AddListener(() => { enabled = true; });
        
        inventory = Inventory.Instance;
    }

    private void FixedUpdate()
    {
        UpdateGroundedStatus();
        if (jumpRequested)
        {
            TryJump();
            jumpRequested = false;
        }
    }

    private void Update()
    {
        UpdateAnimator();
        
        if (CombatStatus.IsDead)
            return;
        
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            TryAttack();
    }

    private IEnumerator RegenerateHp()
    {
        while (true)
        {
            float regenerateAmount = GetExtraStat("HPRecover");
            if (regenerateAmount > 0)
                combatStatus.CurrentHp += regenerateAmount;
            combatStatus.CurrentHp = Mathf.Clamp(combatStatus.CurrentHp, 0, GetMaxHp());
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void UpdateGroundedStatus()
    {
        bool currentlyGrounded = Physics2D.OverlapCircle(
            groundCheckPoint.position, groundCheckRadius, groundLayer);
        
        if (currentlyGrounded && !wasGrounded)
        {
            // 실제로 공중에서 -> 착지한 순간
            jumpCount = 0;
        }
        
        if (currentlyGrounded)
        {
            lastGroundedTime = Time.time;
        }

        isGrounded = currentlyGrounded;
        wasGrounded = currentlyGrounded;
        
        animator.SetBool("isGrounded", isGrounded);
    }
    private void UpdateAnimator()
    {
        animator.SetFloat("speed", Mathf.Abs(playerRigidbody.velocity.x));
        animator.SetFloat("verticalVelocity", playerRigidbody.velocity.y);
    }
    
    private bool IsTouchingWall()
    {
        Vector2 boxCenter = wallCheckPoint.position + Vector3.up * wallCheckYOffset;
        Vector2 boxSize = new Vector2(wallCheckDistance, wallCheckHeight);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0f, wallLayer);
        return hit != null;
    }


    public Damage RollAttack(Weapon weapon = null)
    {
        float str = GetStat(BaseStatType.STR);
        float _int = GetStat(BaseStatType.INT);
        float dex = GetStat(BaseStatType.DEX);
        
        if(weapon == null)
            return new Damage(Mathf.RoundToInt(str), this, DamageType.Physical, false);

        float damage = weapon.GetFinalStat("ATK");

        if (weapon.DamageType == DamageType.Physical)
            damage *= (1 + str / 5.0f);
        else if (weapon.DamageType == DamageType.Magical)
            damage *= (1 + _int / 5.0f);
        else if (weapon.DamageType == DamageType.Ranged)
            damage *= (1 + dex / 5.0f);

        float criticalRate = weapon.GetFinalStat("CriticalRate");
        float criticalDamage = weapon.GetFinalStat("CriticalDamage");
        
        bool isCritical = Random.value < criticalRate;
        if (isCritical)
            damage *= (1 + criticalDamage);
        
        return new Damage(Mathf.RoundToInt(damage), this, weapon.DamageType, isCritical,
                weapon.KnockbackDuration, weapon.KnockbackIntensity, weapon.ParalyzeDuration);
    }

    public void TryAttack()
    {
        if (CombatStatus.IsDead)
            return;
        
        if(currentWeapon == null || currentWeapon.TryAttack())
            animator.SetTrigger("attack");
    }

    // IAttacker 구현
    public void Attack(IDamageable target)
    {
        if (CombatStatus.IsDead)
            return;
        
        Damage damage = RollAttack();

        Debug.Log($"Player dealt {damage.Amount} damage. HP: {combatStatus.CurrentHp}");
        
        Debug.Log(damage);

        if (damage != null)
            target.TakeDamage(damage);
    }

    public void TakeDamage(Damage damage)
    {
        if (CombatStatus.IsDead || isInvulnerable)
            return;
        
        long finalDamage = combatStatus.ApplyDamage(damage);
        
        DamageTextManager.Instance.ShowDamage(finalDamage, damage.IsCritical, Color.red, transform, 2f);

        Debug.Log($"Player took {finalDamage} damage. HP: {combatStatus.CurrentHp}");

        if (combatStatus.IsDead)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player Died");
        
        // 사망 처리
        animator.SetTrigger("death");
        --CombatStatus.life;

        StartCoroutine(TryReviveProgress());
    }

    private IEnumerator TryReviveProgress()
    {
        yield return new WaitForSeconds(3f);
        if (CombatStatus.life > 0)
        {
            animator.SetTrigger("revive");
            isInvulnerable = true;
            combatStatus.CurrentHp = GetMaxHp();
            yield return new WaitForSeconds(invulnerabilityTime);
            animator.SetTrigger("onVulnerable");
            isInvulnerable = false;
        }
        else
        {
            //GameOver
            OnGameOver?.Invoke();
        }
    }
    
    public void Move(Vector2 direction)
    {
        if (CombatStatus.IsDead)
            return;
        
        bool touchingWall = IsTouchingWall();
        float wallDirX = Mathf.Sign(transform.localScale.x); // 벽 체크 방향과 같음 (보통 오른쪽 = +1, 왼쪽 = -1)

        // 벽에 붙었고, 이동 방향이 벽 방향과 같으면 이동 금지
        if (touchingWall && direction.x != 0 && Mathf.Sign(-direction.x) == wallDirX)
        {
            // 벽 방향 이동 차단, 반대 방향 이동은 허용
            direction.x = 0;
        }

        float agi = GetStat(BaseStatType.AGI);
        float moveSpeed = baseMoveSpeed * (0.9f + agi * 0.1f);
        
        Vector2 targetVelocity = new Vector2(direction.x * moveSpeed, playerRigidbody.velocity.y);
        playerRigidbody.velocity = Vector2.Lerp(playerRigidbody.velocity, targetVelocity, 0.2f); // 감속 부드럽게


        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(-direction.x) * Mathf.Abs(scale.x); // 이동 방향으로 캐릭터 바라보게
            transform.localScale = scale;
        }

        //animator.SetFloat("speed", Mathf.Abs(playerRigidbody.velocity.x));
    }

    public void RequestJump()
    {
        jumpRequested = true;
    }

    private void TryJump()
    {
        if (CombatStatus.IsDead) return;
        if (Time.time - lastGroundedTime <= coyoteTime || jumpCount < maxJumpCount)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
            jumpCount++;
            animator.SetTrigger("jump");
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (CombatStatus.IsDead)
            return;
        
        foreach(Transform weaponTransform in WeaponTransform)
        {
            Destroy(weaponTransform.gameObject);
        }

        currentWeapon = Instantiate(weapon);
        weaponInstance = Instantiate(currentWeapon.BaseWeaponPrefab, WeaponTransform);
        currentWeapon.Init(this, weaponInstance);
        SpriteRenderer spriteRenderer = weaponInstance.GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
            spriteRenderer.sprite = weapon.Sprite;
    }

    public void ReceiveReward(Reward reward)
    {
        StatModifiers.Add(StatModifierFactory.CreateModifier(reward.ability, reward.amount));
        UpdateExtraStats();
        CombatStatus.RefreshStat();
        currentWeapon.RefreshWeapon(StatModifiers);
    }

    public void UpdateExtraStats()
    {
        extraStats.Clear();
        foreach (IStatModifier modifier in StatModifiers)
        {
            if (modifier.GetType() == typeof(ExtraStatModifier))
            {
                ExtraStatModifier extraStatModifier = (ExtraStatModifier)modifier;
                if (!extraStats.ContainsKey(extraStatModifier.StatType))
                    extraStats[extraStatModifier.StatType] = 0;

                extraStats[extraStatModifier.StatType] += extraStatModifier.Amount;
            }
        }
    }

    public float GetExtraStat(string statName)
    {
        if(extraStats.ContainsKey(statName))
            return extraStats[statName];

        return 0;
    }

    public float GetStat(BaseStatType statType)
    {
        return CombatStatus.CalculateFinalStat(statType, StatModifiers);
    }

    public long GetMaxHp()
    {
        float vit = GetStat(BaseStatType.VIT);
        float hpPer = GetExtraStat("HPPercentage");

        float baseHp = 50 * (vit + 1);
        return Mathf.RoundToInt(baseHp * (1 + hpPer));
    }

    private void OnDestroy()
    {
        StopCoroutine(RegenerateHp());
        Instance = null;
    }
    private void OnDrawGizmosSelected()
    {
        if (wallCheckPoint == null) return;

        // 박스의 중심과 크기를 계산
        Vector2 boxCenter = wallCheckPoint.position + Vector3.up * wallCheckYOffset;
        Vector2 boxSize = new Vector2(wallCheckDistance, wallCheckHeight);

        // 기즈모 색 설정
        Gizmos.color = Color.red;

        // 박스를 그림 (회전이 없으므로 Quaternion.identity)
        Gizmos.DrawWireCube(boxCenter, boxSize);
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }

}