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
            //Debug.Log(moveInput.x > 0 ? "RightMoving" : "LeftMoving");
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
        var skill = SkillDatabase.Instance.GetSkill("0001_01");
        UnityEngine.Debug.Log("攻撃したンゴ");
        if (skill == null)
        {
            Debug.LogError("SkillDatabase に 0001_01 が存在しません！");
            return;
        }

        skillExecutor.ExecuteSkill(skill, playerParam, playerParam);
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
