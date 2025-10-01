using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class NewEmptyCSharpScript
{
    public enum SkillType
    {
        Attack = 1,
        Move = 2,
        Heal = 3,
        // 将来拡張
    }

    public enum RarityType
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public enum CostType
    {
        None,
        MP,
        Stamina,
        BarrierGauge
    }

    public enum TargetType
    {
        Self,
        SingleEnemy,
        AllEnemies,
        Area
    }

    public enum ConditionType
    {
        None,
        HP,
        Turn,
        Status
    }

    public enum OperatorType
    {
        Equal,
        NotEqual,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual
    }

    [CreateAssetMenu(menuName = "Game/ActiveAbility")]
    public class ActiveAbility : ScriptableObject
    {
        [Header("基本情報")]
        public string SkillCode;
        public string DisplayName;
        public string Description;
        public SkillType SkillType;
        public RarityType Rarity;
        public Sprite Icon;
        public string AnimationKey;

        [Header("効果関連")]
        public List<EffectParameter> EffectParameters = new();
        public float Cooldown;
        public CostType CostType;
        public int CostAmount;
        public TargetType TargetType;

        [Header("成長関連")]
        public int Level = 1;
        public string LevelUpSkillCode;

        [Header("条件式関連")]
        public ConditionType ConditionType;
        public OperatorType Operator;
        public float ConditionValue;

        [Header("装備・管理関連")]
        public int SlotSize = 1;
        public bool CanEquipMultiple = false;
        public bool PermanentAcquire = true;
    }

    [System.Serializable]
    public class EffectParameter
    {
        public string Key;   // "Damage", "Range", "HealAmount" など
        public float Value;
    }

}
