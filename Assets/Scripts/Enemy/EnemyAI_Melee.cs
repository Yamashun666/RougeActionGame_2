using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI_Melee : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 3f;
    public float detectionRange = 8f;  // プレイヤーを感知する範囲
    public float attackRange = 1f;     // 攻撃できる距離
    public float stopDistance = 0.5f;  // 近づきすぎ防止

    [Header("攻撃設定")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;

    private Transform player;
    private Rigidbody2D rb;
    private SkillExecutor skillExecutor;
    private ParameterBase enemyParameter;
    public SkillData attackSkill;

    private bool isAttacking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        skillExecutor = GetComponent<SkillExecutor>();
        enemyParameter = GetComponent<ParameterBase>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 距離判定で行動を分岐
        if (distance <= attackRange)
        {
            // 攻撃範囲 → 攻撃
            TryAttack();
        }
        else if (distance <= detectionRange)
        {
            // 感知範囲 → 追跡
            MoveTowardsPlayer();
        }
        else
        {
            // 範囲外 → 停止
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void MoveTowardsPlayer()
    {
        if (isAttacking) return;

        Vector2 direction = (player.position - transform.position).normalized;

        // 向きを反転
        if (direction.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);

        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 攻撃前ディレイ（溜め動作）
        yield return new WaitForSeconds(0.2f);

        if (skillExecutor != null && attackSkill != null)
        {
            skillExecutor.ExecuteSkill(attackSkill, enemyParameter, null);
            Debug.Log($"[EnemyAI_Melee] {name} が攻撃を実行！");
        }

        // 攻撃後の硬直
        yield return new WaitForSeconds(0.4f);

        isAttacking = false;
    }
}
