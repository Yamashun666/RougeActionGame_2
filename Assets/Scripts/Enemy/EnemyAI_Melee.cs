using UnityEngine;

/// <summary>
/// 近接タイプの基本行動AI
/// </summary>
public class EnemyAI_Melee : EnemyAIController
{
    [Header("AI間隔")]
    public float thinkInterval = 0.3f;
    private float thinkTimer;

    protected override void Think()
    {
        thinkTimer += Time.deltaTime;
        if (thinkTimer < thinkInterval) return;
        thinkTimer = 0f;

        float distance = GetDistanceToPlayer();
        FacePlayer();

        if (distance > detectionRange)
        {
            StopMovement();
            return;
        }

        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMovement();
            TryAttack();
        }
    }

    private void TryAttack()
    {
        SkillData skill = SkillDatabase.Instance.GetSkill("0001_01"); // 通常攻撃
        if (skill == null)
        {
            Debug.LogWarning("[EnemyAI_Melee] 通常攻撃スキルが存在しません。");
            return;
        }

        skillExecutor.ExecuteSkill(skill, parameter, player.GetComponent<ParameterBase>());
        Debug.Log($"[EnemyAI_Melee] {name} がプレイヤーを攻撃！");
    }
}
