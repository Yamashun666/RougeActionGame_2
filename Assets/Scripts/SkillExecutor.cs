using System.Collections;
using UnityEngine;

/// <summary>
/// スキルの実行処理を統括するクラス。
/// SkillDataに基づき、SFX・VFX再生や各種SkillTypeごとの処理を呼び出す。
/// </summary>
public class SkillExecutor : MonoBehaviour
{
    /// <summary>
    /// スキルを実行する。
    /// </summary>
    /// <param name="skill">実行するスキルデータ</param>
    /// <param name="user">スキル使用者</param>
    /// <param name="target">スキル対象</param>
    public void ExecuteSkill(SkillData skill, GameObject user, GameObject target)
    {
        if (skill == null)
        {
            Debug.LogError("SkillData が null です。");
            return;
        }

        // SFX / VFX の再生処理（遅延を考慮してコルーチン化）
        StartCoroutine(PlaySkillEffects(skill));

        // SkillTypeごとの処理（複数同時指定可能）
        if (skill.SkillType001 > 0) ExecuteSkillType((SkillType)skill.SkillType001, skill, user, target, skill.EffectAmount001);
        if (skill.SkillType002 > 0) ExecuteSkillType((SkillType)skill.SkillType002, skill, user, target, skill.EffectAmount002);
        if (skill.SkillType003 > 0) ExecuteSkillType((SkillType)skill.SkillType003, skill, user, target, skill.EffectAmount003);
        if (skill.SkillType004 > 0) ExecuteSkillType((SkillType)skill.SkillType004, skill, user, target, 0);
    }

    /// <summary>
    /// SkillTypeごとに分岐して処理を実行。
    /// </summary>
    void ExecuteSkillType(SkillType type, SkillData skill, GameObject user, GameObject target, int amount)
    {
        switch (type)
        {
            case SkillType.Attack:
                ExecuteAttack(skill, user, target, amount);
                break;

            case SkillType.Move:
                ExecuteMove(skill, user, target, amount);
                break;

            case SkillType.Heal:
                ExecuteHeal(skill, user, target, amount);
                break;

            case SkillType.Buff:
                ExecuteBuff(skill, user, target, amount);
                break;

            default:
                Debug.LogWarning($"未定義のSkillType: {type}");
                break;
        }
    }

    // ===============================================================
    // ▼ 各SkillTypeの実処理（ここにアニメーション・判定処理などを追加していく）
    // ===============================================================

   void ExecuteAttack(SkillData skill, GameObject user, GameObject target, int amount)
{
    var casterParam = user.GetComponent<ParameterBase>();
    var targetParam = target.GetComponent<ParameterBase>();

    if (casterParam == null || targetParam == null)
    {
        Debug.LogWarning("ParameterBase が見つかりません。攻撃スキルを実行できません。");
        return;
    }

    // --- ダメージ計算 ---
    float multiplier = amount / 1000f; // 1000分率
    int rawDamage = Mathf.RoundToInt(casterParam.Attack * multiplier);
    int finalDamage = Mathf.Max(rawDamage - targetParam.Defense, 1);

    // --- ダメージ適用 ---
    targetParam.TakeDamage(finalDamage);

    Debug.Log($"【攻撃スキル】{skill.SkillName} がヒット！ " +
    $"{casterParam.Name} → {targetParam.Name} に {finalDamage} ダメージ！");
}


    void ExecuteMove(SkillData skill, GameObject user, GameObject target, int amount)
    {
        Debug.Log($"【移動スキル】{skill.SkillName} 発動。移動距離: {amount}");
        // TODO: Rigidbody操作・ダッシュアニメ再生などをここに記述
    }

    void ExecuteHeal(SkillData skill, GameObject user, GameObject target, int amount)
    {
        Debug.Log($"【回復スキル】{skill.SkillName} 発動。回復量: {amount}");
        // TODO: HP回復処理（targetのHealthComponentなどに加算）
    }

    void ExecuteBuff(SkillData skill, GameObject user, GameObject target, int amount)
    {
        Debug.Log($"【バフスキル】{skill.SkillName} 発動。効果値: {amount}");
        // TODO: ステータス上昇処理（攻撃力・防御力など）
    }

    // ===============================================================
    // ▼ SFX / VFX の遅延再生処理
    // ===============================================================

    IEnumerator PlaySkillEffects(SkillData skill)
    {
        // --- SFX（効果音） ---
        if (!string.IsNullOrEmpty(skill.UseSkillSFX001))
        {
            yield return new WaitForSeconds(skill.DelayUseSkillSFX001);
            PlaySFX(skill.UseSkillSFX001);
        }

        if (!string.IsNullOrEmpty(skill.UseSkillSFX002))
        {
            yield return new WaitForSeconds(skill.DelayUseSkillSFX002);
            PlaySFX(skill.UseSkillSFX002);
        }

        // --- VFX（視覚エフェクト） ---
        if (!string.IsNullOrEmpty(skill.UseSkillVFX001))
        {
            yield return new WaitForSeconds(skill.DelayUseSkillVFX001);
            PlayVFX(skill.UseSkillVFX001);
        }

        if (!string.IsNullOrEmpty(skill.UseSkillVFX002))
        {
            yield return new WaitForSeconds(skill.DelayUseSkillVFX002);
            PlayVFX(skill.UseSkillVFX002);
        }
    }

    /// <summary>
    /// 効果音を再生する（Resources/SFXフォルダを想定）
    /// </summary>
    void PlaySFX(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"SFX/{fileName}");
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        }
        else
        {
            Debug.LogWarning($"SFXファイルが見つかりません: {fileName}");
        }
    }

    /// <summary>
    /// 視覚エフェクトを生成する（Resources/VFXフォルダを想定）
    /// </summary>
    void PlayVFX(string fileName)
    {
        GameObject vfxPrefab = Resources.Load<GameObject>($"VFX/{fileName}");
        if (vfxPrefab != null)
        {
            Instantiate(vfxPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"VFXファイルが見つかりません: {fileName}");
        }
    }
}
