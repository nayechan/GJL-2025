using System;
using System.Collections;
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
    [SerializeField] private int maxAdditionalJumps = 1;

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance = 0.1f, wallCheckHeight = 1.5f;
    [SerializeField] private float wallCheckYOffset = 0.05f;
    [SerializeField] private Transform wallCheckPoint;

    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private Inventory inventory;

    [SerializeField] private int jumpCount = 0;

    [SerializeField] private CombatStatus combatStatus;
    [SerializeField] private PlayerStatus playerStatus;

    [SerializeField] private bool isGrounded = true;

    [SerializeField] private float invulnerabilityTime = 3.0f;

    private Rigidbody2D playerRigidbody;
    private Animator animator;
    private bool isInvulnerable = false;

    [field: SerializeField] public Transform WeaponTransform { get; private set; }

    public CombatStatus CombatStatus => combatStatus;
    public PlayerStatus PlayerStatus => playerStatus;

    public UnityEvent OnGameOver { get; private set; }

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        OnGameOver = new UnityEvent();
        
        combatStatus.Init();
        playerStatus.Init(combatStatus);

        inventory.UseItemInSlot(0);
    }

    private void Start()
    {
        GameManager.Instance.onPause.AddListener(() => { enabled = false;});
        GameManager.Instance.onResume.AddListener(() => { enabled = true; });
    }

    private void Update()
    {
        UpdateGroundedStatus();
        UpdateAnimator();
        
        if (CombatStatus.IsDead)
            return;
        
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            TryAttack();
    }

    private void UpdateGroundedStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            jumpCount = 0; // 점프 카운트 초기화

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
        long str = combatStatus.baseStats[(int)BaseStatType.STR];
        long _int = combatStatus.baseStats[(int)BaseStatType.INT];
        long dex = combatStatus.baseStats[(int)BaseStatType.DEX];
        
        if(weapon == null)
            return new Damage(str, this, DamageType.Physical);

        long damage = weapon.BaseDamage;

        if (weapon.DamageType == DamageType.Physical)
            damage += (long)(str * DamageUtility.GetEffectiveDamage(weapon.DamageType));
        else if (weapon.DamageType == DamageType.Magical)
            damage += (long)(_int * DamageUtility.GetEffectiveDamage(weapon.DamageType));
        else if (weapon.DamageType == DamageType.Ranged)
            damage += (long)(dex * DamageUtility.GetEffectiveDamage(weapon.DamageType));
        
        return new Damage(damage, this, weapon.DamageType,
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
        
        DamageTextManager.Instance.ShowDamage(finalDamage, Color.red, transform, 2f);

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
            combatStatus.CurrentHp = combatStatus.MaxHp;
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

        float moveSpeed = baseMoveSpeed * (0.9f + combatStatus.baseStats[(int)BaseStatType.AGI] * 0.1f);

        playerRigidbody.velocity = new Vector2(direction.x * moveSpeed, playerRigidbody.velocity.y);

        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(-direction.x) * Mathf.Abs(scale.x); // 이동 방향으로 캐릭터 바라보게
            transform.localScale = scale;
        }

        animator.SetFloat("speed", Mathf.Abs(playerRigidbody.velocity.x));
    }


    public void Jump()
    {
        if (CombatStatus.IsDead)
            return;
        
        if (isGrounded || jumpCount < maxAdditionalJumps)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
            //animator.SetBool("jump", true);
            jumpCount++;
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

        Weapon _weapon = Instantiate(weapon);
        _weapon.Init(this);
        
        GameObject weaponInstance = Instantiate(_weapon.BaseWeaponPrefab, WeaponTransform);

        currentWeapon = _weapon;
        
        weaponInstance.transform.localScale *= weapon.ScaleModifier;
        SpriteRenderer spriteRenderer = weaponInstance.GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
            spriteRenderer.sprite = weapon.Sprite;
    }
}