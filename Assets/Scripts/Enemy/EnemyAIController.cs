using UnityEngine;

/// <summary>
/// 敵AIの基本クラス。全AI共通の参照・基礎機能を提供。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ParameterBase))]
[RequireComponent(typeof(SkillExecutor))]
public abstract class EnemyAIController : MonoBehaviour
{
    [Header("基本参照")]
    public Rigidbody2D rb;
    public ParameterBase parameter;
    public SkillExecutor skillExecutor;
    public Transform player;

    [Header("移動設定")]
    public float moveSpeed = 3f;
    public float detectionRange = 15f;
    public float attackRange = 5f;
    public float stopDistance = 1f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        parameter = GetComponent<ParameterBase>();
        skillExecutor = GetComponent<SkillExecutor>();
    }

    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    protected virtual void Update()
    {
        if (player == null || parameter.CurrentHP <= 0)
            return;

        Think();
    }

    /// <summary>
    /// 各AI固有の思考処理
    /// </summary>
    protected abstract void Think();

    /// <summary>
    /// プレイヤーとの距離を返す
    /// </summary>
    protected float GetDistanceToPlayer()
    {
        return player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;
    }

    /// <summary>
    /// プレイヤーの方向を返す（-1 or +1）
    /// </summary>
    protected float GetDirectionToPlayer()
    {
        if (player == null) return 0f;
        return Mathf.Sign(player.position.x - transform.position.x);
    }

    /// <summary>
    /// プレイヤーに向く
    /// </summary>
    protected void FacePlayer()
    {
        float dir = GetDirectionToPlayer();
        if (dir != 0)
            transform.localScale = new Vector3(dir, 1, 1);
    }

    /// <summary>
    /// 移動（AddForceベース）
    /// </summary>
    protected void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.AddForce(new Vector2(dir.x * moveSpeed, 0f), ForceMode2D.Force);
    }

    /// <summary>
    /// 停止
    /// </summary>
    protected void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
