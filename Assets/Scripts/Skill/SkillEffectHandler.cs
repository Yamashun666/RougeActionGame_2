using UnityEngine;

public class SkillEffectHandler
{
    public void ApplySkillEffect(SkillInstance instance, SkillHitDetector hitDetector)
    {
        // EffectAmount反映
        ApplyEffectAmount(instance.Data.SkillType001, instance.Data, instance.Target);
        ApplyEffectAmount(instance.Data.SkillType002, instance.Data, instance.Target);
        ApplyEffectAmount(instance.Data.SkillType003, instance.Data, instance.Target);
        ApplyEffectAmount(instance.Data.SkillType004, instance.Data, instance.Target);

        // 攻撃スキルならヒット判定
        if (IsAttackSkill(instance.Data))
        {
            hitDetector.PerformHitDetection(instance, instance.Caster.ModelRoot);
        }
    }

    private void ApplyEffectAmount(int skillType, SkillData skill, ParameterBase target)
    {
        switch ((SkillType)skillType)
        {
            case SkillType.Attack:
                target.TakeDamage(skill.EffectAmount001);
                break;
            case SkillType.Move:
                target.MoveSpeed += skill.EffectAmount001;
                break;
            case SkillType.Heal:
                target.Heal(skill.EffectAmount001);
                break;
            case SkillType.Buff:
                target.Attack += skill.EffectAmount001;
                target.Defense += skill.EffectAmount002;
                target.MoveSpeed += skill.EffectAmount003;
                break;
        }
    }

    private bool IsAttackSkill(SkillData skill)
    {
        return skill.SkillType001 == (int)SkillType.Attack ||
               skill.SkillType002 == (int)SkillType.Attack ||
               skill.SkillType003 == (int)SkillType.Attack ||
               skill.SkillType004 == (int)SkillType.Attack;
    }
}
