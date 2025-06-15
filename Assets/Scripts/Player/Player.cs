using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IAttacker, IDamageable, IMovable
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private int maxAdditionalJumps = 1;
    
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private Transform wallCheckPoint;

    
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private float wallSlideSpeed = 1f;
    
    [SerializeField] private CombatStatus combatStatus;
    
    private Rigidbody2D playerRigidbody;
    private Animator animator;

    [SerializeField] private bool isGrounded = true;

    public CombatStatus CombatStatus => combatStatus;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        combatStatus.InitialCallback();
    }


    private void Update()
    {
        UpdateGroundedStatus();
        UpdateAnimator();
        
        if(Input.GetKey(KeyCode.Alpha1))
            CombatStatus.GainExp(1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            CombatStatus.ApplyDamage(new Damage(10,this,DamageType.True));
        
        if (Input.GetMouseButtonDown(0))
            AttackMotion();
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
        Vector2 boxCenter = wallCheckPoint.position;
        Vector2 boxSize = new Vector2(wallCheckDistance, 1.7f); // 높이 1.6f, 너비는 wallCheckDistance
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0f, wallLayer);
        return hit != null;
    }


    public Damage RollAttack()
    {
        return new Damage(combatStatus.attackPower, this, DamageType.Physical);
    }

    public void AttackMotion()
    {
        animator.SetTrigger("attack");
    }

    // IAttacker 구현
    public void Attack(IDamageable target)
    {
        Damage damage = RollAttack();

        Debug.Log($"Player dealt {damage.Amount} damage. HP: {combatStatus.CurrentHp}");
        
        target.TakeDamage(damage);
    }

    public void TakeDamage(Damage damage)
    {
        long finalDamage = combatStatus.ApplyDamage(damage);

        Debug.Log($"Player took {finalDamage} damage. HP: {combatStatus.CurrentHp}");

        if (combatStatus.IsDead)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player Died");
        // 사망 처리
    }
    
    public void Move(Vector2 direction)
    {
        bool touchingWall = IsTouchingWall();
        float wallDirX = Mathf.Sign(transform.localScale.x); // 벽 체크 방향과 같음 (보통 오른쪽 = +1, 왼쪽 = -1)

        // 벽에 붙었고, 이동 방향이 벽 방향과 같으면 이동 금지
        if (touchingWall && direction.x != 0 && Mathf.Sign(-direction.x) == wallDirX)
        {
            // 벽 방향 이동 차단, 반대 방향 이동은 허용
            direction.x = 0;
        }

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
        if (isGrounded || jumpCount < maxAdditionalJumps)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
            //animator.SetBool("jump", true);
            jumpCount++;
        }
    }
}