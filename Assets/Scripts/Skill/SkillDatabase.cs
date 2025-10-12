using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Game/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    public static SkillDatabase Instance { get; private set; }

    [SerializeField]
    private List<SkillData> skills = new List<SkillData>();

    // 初期化
    private void OnEnable()
    {
        Instance = this;
    }

    /// <summary>
    /// SkillCode や SkillName でスキルを取得
    /// </summary>
    public SkillData GetSkill(string skillNameOrCode)
    {
        return skills.Find(s =>
            s.SkillName == skillNameOrCode ||
            s.GroupCode == skillNameOrCode ||
            s.LevelCode == skillNameOrCode);
    }

    /// <summary>
    /// スキル一覧を取得
    /// </summary>
    public List<SkillData> GetAllSkills() => skills;
}
