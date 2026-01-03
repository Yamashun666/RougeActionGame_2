using UnityEngine;

public static class SkillLevelUpCostTable
{
    public static int GetBaseCost(int rarity)
    {
        return rarity switch
        {
            1 => 10, // Common
            2 => 20, // UnCommon
            3 => 30, // Rare
            4 => 40, // Epic
            5 => 50, // Legendary
            _ => 10
        };
    }

    public static int CalculateCost(SkillData skill)
    {
        int baseCost = GetBaseCost(skill.Rarity);
        int level = ExtractLevel(skill.LevelCode);
        return baseCost + (level - 1) * 10;
    }

    private static int ExtractLevel(string levelCode)
    {
        // 例: 0001_03 → 3
        var split = levelCode.Split('_');
        return int.Parse(split[1]);
    }
}

