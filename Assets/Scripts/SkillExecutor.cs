using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitShape
{
    Box,
    Capsule,
    Ray
}

[System.Serializable]
public class SkillInstance
{
    public SkillData Data;
    public ParameterBase Caster;
    public ParameterBase Target;
    public ParameterBase Position;
    public bool IsActive;
    public float Timer;
    public SkillInstance(SkillData data, ParameterBase caster, ParameterBase target)
    {
        Data = data;
        Caster = caster;
        Target = target;
        IsActive = true;
        Timer = 0f;
    }

}

public class SkillExecutor : MonoBehaviour
{
    private List<SkillInstance> activeSkills = new List<SkillInstance>();
    public AudioSource audioSource;
    public Transform effectOrigin;

    // スキル発動
    public void ExecuteSkill(SkillData skill, ParameterBase caster, ParameterBase target)
    {
        SkillInstance instance = new SkillInstance(skill, caster, target);
        activeSkills.Add(instance);

        StartCoroutine(PlaySkillEffects(instance));
        ApplySkillEffect(instance);  // 効果の適用だけ
        HandleSFX(skill);            // SFX処理だけ
        HandleVFX(skill);            // VFX処理だけ
    }
    private void HandleSFX(SkillData skill)
    {
        if (!string.IsNullOrEmpty(skill.UseSkillSFX001))
            StartCoroutine(PlaySFXDelayed(skill.UseSkillSFX001, skill.DelayUseSkillSFX001));
        if (!string.IsNullOrEmpty(skill.UseSkillSFX002))
            StartCoroutine(PlaySFXDelayed(skill.UseSkillSFX002, skill.DelayUseSkillSFX002));
    }
    private void HandleVFX(SkillData skill)
    {
        if (!string.IsNullOrEmpty(skill.UseSkillVFX001))
            StartCoroutine(PlayVFXDelayed(skill.UseSkillVFX001, skill.DelayUseSkillVFX001));
        if (!string.IsNullOrEmpty(skill.UseSkillVFX002))
            StartCoroutine(PlayVFXDelayed(skill.UseSkillVFX002, skill.DelayUseSkillVFX002));
    }
    private IEnumerator PlaySkillEffects(SkillInstance instance)
    {
        // SFX001
        if (!string.IsNullOrEmpty(instance.Data.UseSkillSFX001))
        {
            yield return new WaitForSeconds(instance.Data.DelayUseSkillSFX001);
            Debug.Log($"Play SFX: {instance.Data.UseSkillSFX001}");
        }

        // SFX002
        if (!string.IsNullOrEmpty(instance.Data.UseSkillSFX002))
        {
            yield return new WaitForSeconds(instance.Data.DelayUseSkillSFX002);
            Debug.Log($"Play SFX: {instance.Data.UseSkillSFX002}");
        }

        // VFX001
        if (!string.IsNullOrEmpty(instance.Data.UseSkillVFX001))
        {
            yield return new WaitForSeconds(instance.Data.DelayUseSkillVFX001);
            Debug.Log($"Play VFX: {instance.Data.UseSkillVFX001}");
        }

        // VFX002
        if (!string.IsNullOrEmpty(instance.Data.UseSkillVFX002))
        {
            yield return new WaitForSeconds(instance.Data.DelayUseSkillVFX002);
            Debug.Log($"Play VFX: {instance.Data.UseSkillVFX002}");
        }

        // スキル効果適用
        ApplySkillEffect(instance);
    }
    private IEnumerator PlaySFXDelayed(string sfxName, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioClip clip = Resources.Load<AudioClip>(sfxName);
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
    private IEnumerator PlayVFXDelayed(string vfxName, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject prefab = Resources.Load<GameObject>(vfxName);
        if (prefab != null && effectOrigin != null)
            Instantiate(prefab, effectOrigin.position, Quaternion.identity);
    }
    private void ApplySkillEffect(SkillInstance instance)
    {
        // EffectAmount反映
        ApplyEffectAmount(instance.Data.SkillType001, instance.Data, instance.Target);
        ApplyEffectAmount(instance.Data.SkillType002, instance.Data, instance.Target);
        ApplyEffectAmount(instance.Data.SkillType003, instance.Data, instance.Target);
        ApplyEffectAmount(instance.Data.SkillType004, instance.Data, instance.Target);

        // 攻撃スキルならヒット判定
        if (IsAttackSkill(instance.Data))
        {
            PerformHitDetection(instance);
        }
    }
    // 既存の古い PerformHitDetection は削除
    // 新しい PerformHitDetection に差し替え
    private void PerformHitDetection(SkillInstance instance)
    {
        Vector3 origin = instance.Caster.Position;
        Vector3 direction = (instance.Target.Position - origin).normalized;
        float range = 3f;
        float radius = 0.5f;
        Vector3 halfExtents = Vector3.one;

        HitShape shape = instance.Data.HitShapeType; // スキルごとの判定形状

        Collider[] hitColliders = null;

        switch (shape)
        {
            case HitShape.Box:
                hitColliders = Physics.OverlapBox(origin + direction * range / 2f, halfExtents, Quaternion.identity);
                break;
            case HitShape.Capsule:
                hitColliders = Physics.OverlapCapsule(origin, origin + direction * range, radius);
                break;
            case HitShape.Ray:
                RaycastHit hit;
                if (Physics.Raycast(origin, direction, out hit, range))
                    hitColliders = new Collider[] { hit.collider };
                else
                    hitColliders = new Collider[0];
                break;
        }

        foreach (var col in hitColliders)
        {
            ParameterBase targetParam = col.GetComponent<ParameterBaseHolder>()?.Parameter;
            if (targetParam != null)
            {
                targetParam.TakeDamage(instance.Data.EffectAmount001);
                Debug.Log($"Hit {col.name}, Damage: {instance.Data.EffectAmount001}");
            }
        }
    }


    private void ApplyEffectAmount(int skillType, SkillData skill, ParameterBase target)
    {
        switch ((SkillType)skillType)
        {
            case SkillType.Attack:
                target.CurrentHP = Mathf.Max(target.CurrentHP - skill.EffectAmount001, 0);
                break;
            case SkillType.Move:
                target.MoveSpeed += skill.EffectAmount001;
                break;
            case SkillType.Heal:
                target.CurrentHP = Mathf.Min(target.CurrentHP + skill.EffectAmount001, target.MaxHP);
                break;
            case SkillType.Buff:
                target.Attack += skill.EffectAmount001;
                target.Defense += skill.EffectAmount002;
                target.MoveSpeed += skill.EffectAmount003;
                break;
        }
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

    internal void ExecuteSkill()
    {
        throw new NotImplementedException();
    }
    private bool IsAttackSkill(SkillData skill)
{
    return skill.SkillType001 == (int)SkillType.Attack ||
           skill.SkillType002 == (int)SkillType.Attack ||
           skill.SkillType003 == (int)SkillType.Attack ||
           skill.SkillType004 == (int)SkillType.Attack;
}

}

// 例: ParameterBaseを持たせるラッパー
public class ParameterBaseHolder : MonoBehaviour
{
    public ParameterBase Parameter;
}

