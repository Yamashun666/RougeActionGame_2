using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.SkillSystem;

public class SkillExecutor : MonoBehaviour
{
    private List<SkillInstance> activeSkills = new List<SkillInstance>();
    private SkillHitDetector hitDetector;

    [Header("エフェクト / サウンド")]
    public AudioSource audioSource;
    public Transform effectOrigin;

    private void Awake()
    {
        hitDetector = new SkillHitDetector();
        hitDetector.InitializeLayerMask(); // ← LayerMask初期化をここで安全に行う
    }

    // =============================
    //  スキル発動処理
    // =============================
    public void ExecuteSkill(SkillData skill, ParameterBase caster, ParameterBase target)
    {
        if (skill == null || caster == null)
        {
            Debug.LogWarning("[SkillExecutor] 無効なスキルまたはキャスターが指定されました。");
            return;
        }

        SkillInstance instance = new SkillInstance(skill, caster, target);
        activeSkills.Add(instance);

        StartCoroutine(PlaySkillEffects(instance));
        ApplySkillEffect(instance);
    }

    // =============================
    //  効果適用処理
    // =============================
    private void ApplySkillEffect(SkillInstance instance)
    {
        ApplyEffectAmount(instance.Data.SkillType001, instance.Data, instance.Target);
        ApplyEffectAmount(instance.Data.SkillType002, instance.Data, instance.Target);
        ApplyEffectAmount(instance.Data.SkillType003, instance.Data, instance.Target);
        ApplyEffectAmount(instance.Data.SkillType004, instance.Data, instance.Target);

        if (IsAttackSkill(instance.Data))
        {
            hitDetector.PerformHitDetection(instance, transform);
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

    // =============================
    //  SFX / VFX 管理
    // =============================
    private IEnumerator PlaySkillEffects(SkillInstance instance)
    {
        SkillData skill = instance.Data;

        // SFX
        if (!string.IsNullOrEmpty(skill.UseSkillSFX001))
            yield return StartCoroutine(PlaySFXDelayed(skill.UseSkillSFX001, skill.DelayUseSkillSFX001));
        if (!string.IsNullOrEmpty(skill.UseSkillSFX002))
            yield return StartCoroutine(PlaySFXDelayed(skill.UseSkillSFX002, skill.DelayUseSkillSFX002));

        // VFX
        if (!string.IsNullOrEmpty(skill.UseSkillVFX001))
            yield return StartCoroutine(PlayVFXDelayed(skill.UseSkillVFX001, skill.DelayUseSkillVFX001));
        if (!string.IsNullOrEmpty(skill.UseSkillVFX002))
            yield return StartCoroutine(PlayVFXDelayed(skill.UseSkillVFX002, skill.DelayUseSkillVFX002));
    }

    private IEnumerator PlaySFXDelayed(string sfxName, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioClip clip = Resources.Load<AudioClip>(sfxName);
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"[SFX] {sfxName} が見つからないか AudioSource が未設定です。");
    }

    private IEnumerator PlayVFXDelayed(string vfxName, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject prefab = Resources.Load<GameObject>(vfxName);
        if (prefab != null && effectOrigin != null)
            Instantiate(prefab, effectOrigin.position, Quaternion.identity);
        else
            Debug.LogWarning($"[VFX] {vfxName} が見つからないか effectOrigin が未設定です。");
    }

    // =============================
    //  クールタイム管理
    // =============================
    private void Update()
    {
        for (int i = activeSkills.Count - 1; i >= 0; i--)
        {
            SkillInstance inst = activeSkills[i];
            if (!inst.IsActive)
            {
                activeSkills.RemoveAt(i);
                continue;
            }

            inst.Timer += Time.deltaTime;

            if (inst.Timer >= inst.Data.CoolTime / 1000f)
            {
                inst.IsActive = false;
                Debug.Log($"[SkillExecutor] {inst.Data.SkillName} のクールタイム終了");
            }
        }
    }

}
