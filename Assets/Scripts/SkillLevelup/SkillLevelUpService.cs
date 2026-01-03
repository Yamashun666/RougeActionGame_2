using UnityEngine;

public class SkillLevelUpService : MonoBehaviour
{
    public SkillDatabase skillDatabase;
    public PlayerInventory inventory;

    public void TryLevelUp(SkillSlotUI slot)
    {
        SkillData current = slot.assignedSkill;
        if (current == null) return;

        if (string.IsNullOrEmpty(current.LevelUPSkillCode))
        {
            Debug.Log("最大レベルです");
            return;
        }

        SkillData next = skillDatabase.GetSkill(current.LevelUPSkillCode);
        if (next == null)
        {
            Debug.LogError("LevelUPSkillCode が不正");
            return;
        }

        int cost = SkillLevelUpCostTable.CalculateCost(current);

        if (!inventory.CanPay(cost))
        {
            Debug.Log("コスト不足");
            return;
        }

        // 支払い
        inventory.Pay(cost);

        // 差し替え
        slot.ReplaceSkill(next);

        Debug.Log($"LvUp成功: {current.SkillName} → {next.SkillName}");
    }
}

