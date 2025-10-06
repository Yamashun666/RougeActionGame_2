using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;


    [Header("Attack Settings")]
    public SkillExecutor skillExecutor;
    public SkillData defaultAttackSkill; // 仮の攻撃スキル処理
    public ParameterBase playerParameter;
    public ParameterBase enemyParameter; // ※仮のターゲット
    private Animator animator;
    private float moveInput;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;
    public KeyCode AttackKey = KeyCode.J; // 攻撃ボタン
    public SkillData DefaultAttackSkill;  // デフォルト攻撃スキル
    public ParameterBase PlayerParameter;  // プレイヤーのステータス

        // 攻撃判定用
    public Vector2 attackBoxSize = new Vector2(1f, 1f);
    public Vector2 attackBoxOffset = new Vector2(1f, 0f);
    public LayerMask enemyLayer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
        HandleAnimation();

    }
        void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        HandleMovement();
        HandleJump();
        HandleAttack();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
        void HandleAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    // 横移動処理
    void HandleMovement()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // 向きの反転
        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();
    }


    // ジャンプ処理
    void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

        private void OnCollisionEnter2D(Collision2D collision)
    {
        // 地面判定
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // 攻撃処理
    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                ExecuteDefaultAttack();
            }
        }
    }

    private void ExecuteDefaultAttack()
    {
        // スキル本体の処理
        skillExecutor.ExecuteSkill(defaultAttackSkill, playerParameter, enemyParameter);

        // 攻撃判定（Box型）
        Vector2 boxCenter = (Vector2)transform.position + attackBoxOffset;
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, attackBoxSize, 0f, enemyLayer);

        foreach (var hit in hits)
        {
            Damageable enemy = hit.GetComponent<Damageable>();
            if (enemy != null)
            {
                enemy.TakeDamage(DefaultAttackSkill.EffectAmount001);
                Debug.Log($"[Damage] {enemy.name} に {DefaultAttackSkill.EffectAmount001} ダメージ");
            }
        }

        // 簡易ログとエフェクト呼び出し
        Debug.Log($"[Skill Test] {DefaultAttackSkill.SkillName} 発動!");
        Debug.Log($"EffectAmount001: {DefaultAttackSkill.EffectAmount001}");

        // エフェクトやSFXはSkillExecutor内で処理させる想定
    }
    // 向きを反転
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}



