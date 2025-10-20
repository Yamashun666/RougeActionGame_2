using UnityEngine;

[RequireComponent(typeof(ParameterBase))]
public class EnemyAIController : MonoBehaviour
{
    [Header("ターゲット設定")]
    public Transform player;             // プレイヤー（自動追尾対象）

    [Header("移動パラメータ")]
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;    // これ以下で停止

    [Header("攻撃設定")]
    public SkillData defaultAttackSkill; // 使用する攻撃スキル
    public float attackRange = 2f;
    public float attackCooldown = 2f;    // 攻撃間隔（秒）

    private ParameterBase enemyParam;
    private SkillExecutor skillExecutor;
    private float attackTimer;

    private void Awake()
    {
        enemyParam = GetComponent<ParameterBase>();
        skillExecutor = FindObjectOfType<SkillExecutor>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // 死亡してたら動かない
        if (enemyParam.CurrentHP <= 0) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 攻撃範囲外 → 追いかける
        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // 攻撃範囲内 → 攻撃処理
            HandleAttack(distance);
        }

        attackTimer += Time.deltaTime;
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    private void HandleAttack(float distance)
    {
        if (attackTimer >= attackCooldown && defaultAttackSkill != null)
        {
            skillExecutor.ExecuteSkill(defaultAttackSkill, enemyParam, enemyParam); // 仮で自己参照
            Debug.Log($"Enemy attacks player with {defaultAttackSkill.SkillName}");
            attackTimer = 0f;
        }
    }
}
