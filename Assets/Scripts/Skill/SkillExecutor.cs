using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.SkillSystem; // HitShape 用

/// <summary>
/// スキルの実行を統括管理するクラス。
/// 発動 → 演出呼び出し → 効果適用 → クールタイム監視。
/// </summary>
public class SkillExecutor : MonoBehaviour
{
    private List<SkillInstance> activeSkills = new List<SkillInstance>();
    public AudioSource audioSource;
    public Transform effectOrigin;
    private SkillHitDetector hitDetector = new SkillHitDetector();

    public void ExecuteSkill(SkillData skill, ParameterBase caster, ParameterBase target)
    {
        SkillInstance instance = new SkillInstance(skill, caster, target);
        activeSkills.Add(instance);

        StartCoroutine(PlaySkillEffects(instance));
        ApplySkillEffect(instance); // 効果適用
    }

    private IEnumerator PlaySkillEffects(SkillInstance instance)
    {
        SkillData skill = instance.Data;

        // SFX & VFX 再生
        yield return StartCoroutine(PlaySFXDelayed(skill.UseSkillSFX001, skill.DelayUseSkillSFX001));
        yield return StartCoroutine(PlaySFXDelayed(skill.UseSkillSFX002, skill.DelayUseSkillSFX002));
        yield return StartCoroutine(PlayVFXDelayed(skill.UseSkillVFX001, skill.DelayUseSkillVFX001));
        yield return StartCoroutine(PlayVFXDelayed(skill.UseSkillVFX002, skill.DelayUseSkillVFX002));

        // 最後に効果適用
        ApplySkillEffect(instance);
    }

    private IEnumerator PlaySFXDelayed(string sfxName, float delay)
    {
        if (string.IsNullOrEmpty(sfxName)) yield break;

        yield return new WaitForSeconds(delay);
        AudioClip clip = Resources.Load<AudioClip>(sfxName);
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    private IEnumerator PlayVFXDelayed(string vfxName, float delay)
    {
        if (string.IsNullOrEmpty(vfxName)) yield break;

        yield return new WaitForSeconds(delay);
        GameObject prefab = Resources.Load<GameObject>(vfxName);
        if (prefab != null && effectOrigin != null)
            Instantiate(prefab, effectOrigin.position, Quaternion.identity);
    }

    private void ApplySkillEffect(SkillInstance instance)
    {
        SkillData skill = instance.Data;
        ParameterBase target = instance.Target;

        // スキルタイプごとに反映
        ApplyEffectAmount(skill.SkillType001, skill, target);
        ApplyEffectAmount(skill.SkillType002, skill, target);
        ApplyEffectAmount(skill.SkillType003, skill, target);
        ApplyEffectAmount(skill.SkillType004, skill, target);

        // 攻撃スキルならヒット判定
        if (IsAttackSkill(skill))
        {
            hitDetector.PerformHitDetection(instance);
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

    private void Update()
    {
        for (int i = activeSkills.Count - 1; i >= 0; i--)
        {
            SkillInstance inst = activeSkills[i];
            if (!inst.IsActive) { activeSkills.RemoveAt(i); continue; }

            inst.Timer += Time.deltaTime;
            if (inst.Timer >= inst.Data.CoolTime / 1000f)
            {
                inst.IsActive = false;
                Debug.Log($"Skill {inst.Data.SkillName} finished");
            }
        }
    }
}
