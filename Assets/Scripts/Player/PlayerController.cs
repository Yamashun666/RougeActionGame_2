using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 8f;
    public float jumpForce = 20f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    [Header("参照")]
    public ParameterBase parameter;
    public SkillExecutor skillExecutor;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool jumpQueued;
    private ParameterBase playerParam;
    public SkillHitDetector hitDetector;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerParam = GetComponent<ParameterBase>();

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        // 入力イベント登録
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;
        inputActions.Player.Jump.performed += _ => jumpQueued = true;
        inputActions.Player.Attack.performed += _ => HandleAttack();

        SkillDatabase.Initialize();
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        //UnityEngine.Debug.Log(isGrounded);
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        // AddForce方式を維持して、InputSystem入力を反映
        if (moveInput.x != 0)
        {
            Vector2 moveForce = new Vector2(moveInput.x * moveSpeed, 0f);
            rb.AddForce(moveForce, ForceMode2D.Force);
            if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * moveSpeed, rb.linearVelocity.y);
            }
            if (moveInput.x == 0)
            {
    // 減速率
                float decelFactor = 0.85f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * decelFactor, rb.linearVelocity.y);

                // 速度がかなり小さければ完全停止
                if (Mathf.Abs(rb.linearVelocity.x) < 0.1f)
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    void HandleJump()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (jumpQueued && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpQueued = false;
        }

    }

    private void HandleAttack()
    {
        SkillDatabase.Initialize();
        var skill = SkillDatabase.Instance.GetSkill("0001_01");
        if (skill == null)
        {
            Debug.LogError("[HandleAttack] Skill 0001_01 not found");
            return;
        }

        if (skillExecutor == null)
        {
            Debug.LogError("[HandleAttack] SkillExecutor 未設定");
            return;
        }

        if (playerParam == null)
        {
            Debug.LogError("[HandleAttack] playerParam 未設定");
            return;
        }

        skillExecutor.ExecuteSkill(skill, playerParam, playerParam);
        Debug.Log("[HandleAttack] 攻撃スキル発動中");
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }
}
