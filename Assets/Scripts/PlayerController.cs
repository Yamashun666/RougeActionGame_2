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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
        HandleInput();
        HandleAnimation();
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

    void HandleAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        // 攻撃入力
        if (Input.GetKeyDown(AttackKey) && DefaultAttackSkill != null)
        {
            // 攻撃スキルを実行
            skillExecutor.ExecuteSkill(defaultAttackSkill, playerParameter, enemyParameter);
        }
    }

    // 攻撃処理
    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (skillExecutor != null && defaultAttackSkill != null)
            {
                // 攻撃実行
                skillExecutor.ExecuteSkill(defaultAttackSkill, playerParameter, enemyParameter);
                Debug.Log("Attack executed!");
            }
        }
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


