using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Database/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    // ======== シングルトン管理 ========
    public static SkillDatabase Instance { get; private set; }

    // ======== 登録スキル一覧 ========
    [Header("登録されているスキルリスト")]
    public List<SkillData> skills = new List<SkillData>();

    // ======== 初期化処理 ========
    private void OnEnable()
    {
        // ScriptableObjectがロードされた際にInstanceを登録
        Instance = this;
    }

    /// <summary>
    /// 手動初期化関数（Resourcesからロード）
    /// </summary>
    public static void Initialize()
    {
        if (Instance != null)
        {
            Debug.Log("[SkillDatabase] すでに初期化済みです。");
            return;
        }

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

    /// <summary>
    /// スキル名からスキルデータを取得
    /// </summary>
    public SkillData GetSkill(string skillName)
    {
        SkillData result = skills.Find(s => s.SkillName == skillName);
        if (result == null)
        {
            Debug.LogWarning($"[SkillDatabase] Skill '{skillName}' が見つかりません。");
        }
        return result;
    }
}
