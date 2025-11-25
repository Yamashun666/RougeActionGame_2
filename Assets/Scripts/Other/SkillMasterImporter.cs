using UnityEngine;
using UnityEditor;
using System.IO;

public class SkillMasterImporter : EditorWindow
{
    [MenuItem("Tools/Import Skill CSV")]
    public static void ImportSkills()
    {
        string csvPath = "Assets/Data/SkillMaster.csv";
        if (!File.Exists(csvPath))
        {
            Debug.LogError($"CSVファイルが見つかりません: {csvPath}");
            return;
        }

        string[] lines = File.ReadAllLines(csvPath);
        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(',');

            SkillData skill = ScriptableObject.CreateInstance<SkillData>();
            skill.SkillName = cols[0];
            skill.GroupCode = cols[1];
            skill.LevelCode = cols[2];
            skill.Rarity = int.Parse(cols[3]);
            skill.CoolTime = int.Parse(cols[4]);
            skill.Type = (SkillType)System.Enum.Parse(typeof(SkillType), cols[6]);
            skill.EffectAmount001 = int.Parse(cols[7]);
            skill.SkillIcon = cols[9];
            skill.DropPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + cols[8] + ".prefab");

            string assetPath = $"Assets/ScriptableObjects/Skills/{skill.SkillName}.asset";
            AssetDatabase.CreateAsset(skill, assetPath);
            Debug.Log($"✅ {skill.SkillName} を生成しました。");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 SkillMaster CSV インポート完了");
    }
}
