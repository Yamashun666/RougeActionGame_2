using UnityEngine;
using System.Linq;

/// <summary>
/// ガンビット型の条件AI（EnemyAIBaseに付随して上書き）
/// </summary>
public class EnemyAI_Gambit : EnemyAIController

{
    public EnemyGambitSet gambitSet;
    public float evaluateInterval = 0.5f;

    protected override void Think()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            EvaluateGambits();
            TimerSetter();
        }
    }

    private void EvaluateGambits()
    {
        Debug.Log("[EnemyGambitSet.EvaluateGambits]Called EvaluateGambits");
        if (gambitSet == null || gambitSet.gambits.Count == 0)
            return;

        foreach (var g in gambitSet.gambits.OrderByDescending(g => g.priority))
        {
            if (EvaluateCondition(g))
            {
                ExecuteAction(g);
                break;
            }
        }
    }

    private bool EvaluateCondition(EnemyGambitSO gambit)
    {
        float distance = GetDistanceToPlayer();
        float hpRate = parameter.CurrentHP / (float)parameter.MaxHP;

        switch (gambit.flagNum)
        {
            case ConditionFlag.HPPercent:
                return hpRate <= gambit.targetPoint / 100f;

            case ConditionFlag.Distance:
                return distance <= gambit.targetPoint;

            default:
                return false;
        }
    }

    private void ExecuteAction(EnemyGambitSO gambit)
    {
        SkillData skill = SkillDatabase.Instance.GetSkill(gambit.callActionCode);
        if (skill == null)
        {
            Debug.LogWarning($"[EnemyAI_Gambit] スキルコード {gambit.callActionCode} が見つかりません。");
            return;
        }

        skillExecutor.ExecuteSkill(skill, parameter, player.GetComponent<ParameterBase>());
        Debug.Log($"[EnemyAI_Gambit] {name} が {skill.SkillName} を発動（条件：{gambit.gambitName}）");
    }
}
