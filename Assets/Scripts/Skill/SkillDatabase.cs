using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Database/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    public static SkillDatabase Instance { get; private set; }

    [Header("登録されているスキルリスト")]
    public List<SkillData> skills = new List<SkillData>();

    private void OnEnable()
    {
        Instance = this;
    }

    public static void Initialize()
    {
        if (Instance != null) return;

        SkillDatabase db = Resources.Load<SkillDatabase>("SkillDatabase");
        if (db != null)
        {
            Instance = db;
            Debug.Log("[SkillDatabase] Resources からロード成功。");
        }
        else
        {
            Debug.LogError("[SkillDatabase] SkillDatabase.asset が Resources に存在しません！");
        }
    }
    public IReadOnlyList<SkillData> GetAllSkills()
    {
        return skills;
    }
    public SkillData GetRandomSkillByRarity(int rarity, bool excludeUnique = true)
    {
        List<SkillData> candidates = new List<SkillData>();

        foreach (var skill in skills)
        {
            if (skill == null) continue;
            if (skill.Rarity != rarity) continue;
            if (excludeUnique && skill.IsUnique) continue;

            candidates.Add(skill);
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"[SkillDatabase] 該当レアリティ({rarity})のスキルが見つかりません。");
            return null;
        }

        int index = Random.Range(0, candidates.Count);
        return candidates[index];
    }

    /// <summary>
    /// LevelCodeでスキルを検索
    /// </summary>
    public SkillData GetSkill(string levelCode)
    {
        SkillData result = skills.Find(s => s.LevelCode == levelCode);
        if (result == null)
        {
            Debug.LogWarning($"[SkillDatabase] Skill(LevelCode='{levelCode}') が見つかりません。");
        }
        return result;
    }
}
