using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("所持中のアイテム")]
    public List<DroppedItem> items = new List<DroppedItem>();

    [Header("所持中のスキル")]
    public List<SkillData> skills = new List<SkillData>();

    /// <summary>
    /// アイテムを追加
    /// </summary>
    public void AddItem(DroppedItem droppedItem)
    {
        if (droppedItem == null)
        {
            Debug.LogWarning("[InventoryManager] AddItem: droppedItem が null です");
            return;
        }

        items.Add(droppedItem);
        Debug.Log($"[InventoryManager] {droppedItem.name} を追加しました。");
    }

    /// <summary>
    /// スキルを追加（重複ならレベルアップを呼ぶ）
    /// </summary>
    public void AddSkill(SkillData skill)
    {
        if (skill == null) return;

        var existing = skills.Find(s => s.GroupCode == skill.GroupCode);
        if (existing != null)
        {
            Debug.Log($"[InventoryManager] {skill.SkillName} はすでに所持中 → レベルアップ処理へ");
            // TODO: スキルレベルアップ処理を呼ぶ
        }
        else
        {
            skills.Add(skill);
            Debug.Log($"[InventoryManager] 新スキル {skill.SkillName} を獲得！");
        }
    }
}
