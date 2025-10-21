using System;
using System.Collections.Generic;
using UnityEngine;
using Game.SkillSystem;

public enum SkillType
{
    Attack = 1,
    Move = 2,
    Heal = 3,
    Buff = 4
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public string SkillName;            // スキルの名称
    public string GroupCode;            // レベルすべてを包括したスキルのCODE
    public string LevelCode;            // GroupCodeをスキルレベルごとに分割したCODE
    public int Rarity;                  // このスキルのレアリティ。1=コモン 2=アンコモン 3 = レア 4 = エピック 5 = レジェンダリー
    public int SkillType001;
    public int SkillType002;
    public int SkillType003;
    public int SkillType004;
    public SkillType Type;
    public int EffectAmount001;
    public int EffectAmount002;
    public int EffectAmount003;
    public int CoolTime;
    public int LevelUP_LevelCode;
    public string UseSkillSFX001;
    public float DelayUseSkillSFX001;
    public string UseSkillSFX002;
    public float DelayUseSkillSFX002;
    public string UseSkillVFX001;
    public float DelayUseSkillVFX001;
    public string UseSkillVFX002;
    public float DelayUseSkillVFX002;
    public string SkillEnhancementTable;
    public string SkillIcon;
    public string LevelUPSkillCode;
    public HitShape HitShapeType;

    [Header("特殊設定")]
    public bool IsUnique = false;
}

//////////////////////////////////////////////////////////
/// ここから SkillManager の定義 //////////////////////////
//////////////////////////////////////////////////////////

public class SkillManager : MonoBehaviour
{
    [Header("プレイヤーが保持しているスキル一覧")]
    public List<SkillData> ownedSkills = new List<SkillData>();

    /// <summary>
    /// スキルを追加する。
    /// 既に同じGroupCodeを持つスキルがある場合はレベルアップ判定。
    /// </summary>
    public void AddSkill(SkillData newSkill)
    {
        if (newSkill == null)
        {
            Debug.LogWarning("[SkillManager] nullスキルをAddSkillに渡しました。");
            return;
        }

        // 重複チェック
        SkillData existing = ownedSkills.Find(s => s.GroupCode == newSkill.GroupCode);
        if (existing != null)
        {
            HandleLevelUp(existing);
            return;
        }

        // 新規追加
        ownedSkills.Add(newSkill);
        Debug.Log($"🆕 スキル [{newSkill.SkillName}] を新たに習得！");
    }

    /// <summary>
    /// スキルのレベルアップ処理。
    /// </summary>
    private void HandleLevelUp(SkillData existing)
    {
        if (string.IsNullOrEmpty(existing.LevelUPSkillCode))
        {
            Debug.Log($"🔸 [{existing.SkillName}] は最大レベルです。");
            return;
        }

        SkillData nextLevel = SkillDatabase.Instance.GetSkill(existing.LevelUPSkillCode);
        if (nextLevel == null)
        {
            Debug.LogWarning($"[SkillManager] LevelUPSkillCode='{existing.LevelUPSkillCode}' が見つかりません。");
            return;
        }

        ownedSkills.Remove(existing);
        ownedSkills.Add(nextLevel);
        Debug.Log($"⚡ スキル [{existing.SkillName}] → [{nextLevel.SkillName}] にレベルアップ！");
    }

    /// <summary>
    /// 指定スキルを削除（将来用）
    /// </summary>
    public void RemoveSkill(SkillData skill)
    {
        if (ownedSkills.Contains(skill))
        {
            ownedSkills.Remove(skill);
            Debug.Log($"❌ スキル [{skill.SkillName}] を削除しました。");
        }
    }
}
