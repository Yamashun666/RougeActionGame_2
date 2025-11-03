using System.Collections;
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
    public SkillData skillData;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    public bool isGrounded;
    private bool jumpQueued;
    private ParameterBase playerParam;
    public SkillHitDetector hitDetector;
    private bool canDoubleJump = false;  // 今「一度だけ」二段ジャンプができる状態か
    private bool hasUsedDoubleJump = false; // 既に使ったかどうか
    public Transform footVFXAnchor;
    private bool isStepBackActive = false; // ステップ中フラグ
    private float stepBackDuration = 0.3f;   // ステップ時間（SkillDataから受け取ってもOK）
    private bool isJetBoosting = false;  // いまブースト中か（実行状態）
    public bool hasJetBoost = false;
    public SkillData jetBoostSkill;      // JetBoost用のSkillData参照
    public Transform magicOrigin;
    private Animator animator;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerParam = GetComponent<ParameterBase>();
        animator = GetComponent<Animator>();

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
        HandleMovement();
        HandleJump();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        if (isStepBackActive) return;

        if (moveInput.x != 0)
        {
            // 進行方向に向きを反転
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveInput.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;

            Vector2 moveForce = new Vector2(moveInput.x * moveSpeed, 0f);
            rb.AddForce(moveForce, ForceMode2D.Force);

            if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed)
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            float decelFactor = 0.85f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * decelFactor, rb.linearVelocity.y);
            if (Mathf.Abs(rb.linearVelocity.x) < 0.1f)
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
    private void UpdateAnimator()
    {
        if (animator == null) return;

        float speed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);

        // ジャンプ or 落下状態
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.1f)
                animator.SetBool("IsJumping", true);
            else
                animator.SetBool("IsJumping", false);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }
    }

    public void StartJetBoost(float thrustPower, float gravityScale, float duration)
    {
        if (isJetBoosting) return;     // 多重起動防止
        StartCoroutine(JetBoostRoutine(thrustPower, gravityScale, duration));
    }

    private IEnumerator JetBoostRoutine(float thrustPower, float gravityScale, float duration)
    {
        isJetBoosting = true;

        float originalGravity = rb.gravityScale;
        float originalDrag = rb.linearDamping;

        rb.gravityScale = gravityScale;
        rb.linearDamping = 0.5f; // 上昇中の初期値

        float elapsed = 0f;
        float maxUpVelocity = 12f;

        Debug.Log($"[JetBoost] 開始: thrust={thrustPower}, gravityScale={gravityScale}, duration={duration}");

        rb.AddForce(Vector2.up * thrustPower * 0.8f, ForceMode2D.Impulse);

        while (elapsed < duration)
        {
            if (inputActions.Player.Jump.IsPressed())
            {
                if (rb.linearVelocity.y < maxUpVelocity)
                    rb.AddForce(Vector2.up * thrustPower * 0.15f, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("[JetBoost] ジャンプキー離し → ブースト解除＆落下");
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 🪂 滞空フェーズ: dragを0.5→1.0にゆっくり補間
        float dragDuration = 0.5f;
        float dragElapsed = 0f;

        while (dragElapsed < dragDuration)
        {
            rb.linearDamping = Mathf.Lerp(0.5f, 1.0f, dragElapsed / dragDuration);
            dragElapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearDamping = 1.0f; // 最終 drag 確定
        yield return new WaitForSeconds(0.3f);

        // 🔽 徐々に重力を戻す
        rb.gravityScale = Mathf.Lerp(rb.gravityScale, originalGravity, 0.5f);
        yield return new WaitForSeconds(0.2f);

        rb.gravityScale = originalGravity;
        rb.linearDamping = originalDrag;
        isJetBoosting = false;

        Debug.Log("[JetBoost] 終了（drag戻す・重力戻す）");
    }
    public void EnableTemporaryDoubleJump(float duration = 5f)
    {
        StopAllCoroutines(); // 複数スキル重複対策
        StartCoroutine(DoubleJumpEnableRoutine(duration));
    }

    private IEnumerator DoubleJumpEnableRoutine(float duration)
    {
        canDoubleJump = true;
        hasUsedDoubleJump = false;
        Debug.Log($"[Player] 二段ジャンプ解禁！（{duration}秒間）");

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        canDoubleJump = false;
        Debug.Log("[Player] 二段ジャンプ効果が終了しました。");
    }
    void HandleJump()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (jumpQueued)
        {
            jumpQueued = false; // 入力消費

            // JetBoost装備中ならジャンプを置換
            if (hasJetBoost && jetBoostSkill != null)
            {
                // JetBoostを発動する（地上のみ）
                if (isGrounded && !isJetBoosting)
                {
                    float thrust   = (float)jetBoostSkill.EffectAmount001;
                    float grav     = jetBoostSkill.EffectAmount002 > 0 ? jetBoostSkill.EffectAmount002 / 100f : 0.5f;
                    float duration = jetBoostSkill.EffectAmount003 > 0 ? jetBoostSkill.EffectAmount003 : 2f;

                    StartJetBoost(thrust, grav, duration);
                    Debug.Log("[HandleJump] JetBoost 発動");
                }
                else
                {
                    Debug.Log("[HandleJump] JetBoost中 or 空中 → 通常ジャンプ抑制");
                }
            }
            else
            {
                // JetBoostを持っていないなら通常ジャンプ
                if (isGrounded)
                {
                    Jump();
                    hasUsedDoubleJump = false;
                }
                else if (canDoubleJump && !hasUsedDoubleJump)
                {
                    DoubleJump(skillData);
                }
            }
        }
    }


    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // 上昇速度をリセットして安定化
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Debug.Log("🟩 通常ジャンプ");
    }
    public void DoubleJump(SkillData skill)
    {
        hasUsedDoubleJump = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce * 0.9f, ForceMode2D.Impulse);
        SkillEffectPlayer.Instance.PlaySkillEffects(skill, transform);

        Debug.Log("🟢 スキルによる二段ジャンプ発動！");
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
        animator?.SetTrigger("Attack");
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
    public void PerformStepBack(float distance, float power)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        // 入力を無効化
        if (!isStepBackActive)
            StartCoroutine(StepBackRoutine(distance, power));
    }


    private IEnumerator StepBackRoutine(float distance, float power)
    {
        isStepBackActive = true;

        // 現在の向きに応じて反対方向へAddForce
        float dir = Mathf.Sign(transform.localScale.x);
        Vector2 stepDir = new Vector2(-dir, 0);

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);  // 現在の横移動をリセット
        float forceAmount = distance * power;
        rb.AddForce(stepDir * forceAmount, ForceMode2D.Impulse);

        Debug.Log($"[StepBack] AddForce dir={stepDir}, force={forceAmount}");

        // ステップ中の入力を一時無効化
        yield return new WaitForSeconds(stepBackDuration);

        isStepBackActive = false;
        Debug.Log("[StepBack] 終了（入力再開）");
    }
}
