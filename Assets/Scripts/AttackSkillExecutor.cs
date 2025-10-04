using UnityEngine;

public class AttackSkillExecutor : MonoBehaviour
{
    public SkillData skillData;              // 使用するスキルデータ
    public ParameterBase casterParam;        // 攻撃者のパラメータ
    public float hitRange = 2.0f;            // 攻撃判定の半径
    public LayerMask targetLayer;            // 対象レイヤー（例: "Enemy"）

    public void UseSkill()
    {
        // ここでアニメーション・SFX/VFX 再生処理を呼び出してもOK
        Debug.Log($"スキル発動: {skillData.SkillName}");

        // 実際の攻撃判定処理
        PerformAttackHit();
    }

    private void PerformAttackHit()
    {
        // 中心点をキャラクター前方にずらす（少し前に判定出す）
        Vector3 center = transform.position + transform.forward * hitRange * 0.5f;

        // 半径内のコライダーをすべて取得
        Collider[] hitColliders = Physics.OverlapSphere(center, hitRange, targetLayer);

        foreach (var hit in hitColliders)
        {
            ParameterBase targetParam = hit.GetComponent<ParameterBase>();
            if (targetParam != null && targetParam != casterParam)
            {
                int amount = skillData.EffectAmount001; // 例: 攻撃倍率
                ExecuteAttack(skillData, casterParam, targetParam, amount);
            }
        }
    }

    private void ExecuteAttack(SkillData skill, ParameterBase caster, ParameterBase target, int amount)
    {
        float multiplier = amount / 1000f; // 1000分率で倍率計算
        int rawDamage = Mathf.RoundToInt(caster.Attack * multiplier);
        int finalDamage = Mathf.Max(rawDamage - target.Defense, 1);

        target.TakeDamage(finalDamage);

        Debug.Log($"【攻撃スキル】{skill.SkillName}: {caster.Name} → {target.Name} に {finalDamage} ダメージ！");
    }

    // 攻撃範囲をScene上で可視化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + transform.forward * hitRange * 0.5f;
        Gizmos.DrawWireSphere(center, hitRange);
    }
}
