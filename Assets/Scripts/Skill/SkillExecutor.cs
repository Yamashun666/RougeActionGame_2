using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.SkillSystem;

public class SkillExecutor : MonoBehaviour
{
    public int lastEffectAmount;
    private List<SkillInstance> activeSkills = new List<SkillInstance>();
    [Header("SkillHitDetector")]
    public SkillHitDetector hitDetector;

    [Header("エフェクト / サウンド")]
    public AudioSource audioSource;
    public Transform effectOrigin;
    public PlayerController playerController;


    private void Start()
    {
        hitDetector = GetComponent<SkillHitDetector>();
        Debug.Log($"[SkillExecutor] hitDetector取得確認: {(hitDetector == null ? "null" : hitDetector.name)}");
        playerController = GetComponent<PlayerController>();
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
    public void ExecuteDoubleJump(SkillData skill, ParameterBase caster)
    {
        var player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        player.EnableTemporaryDoubleJump();
        Debug.Log("[SkillExecutor] 二段ジャンプスキルを発動！");
    }

    // =============================
    //  効果適用処理
    // =============================
    private void ApplySkillEffect(SkillInstance instance)
    {
        if (instance == null || instance.Data == null)
        {
            Debug.LogError("[SkillExecutor] instance または Data が null です。");
            return;
        }

        // ターゲットが設定されていない場合、ヒット判定で見つける方式に切り替える
        Damageable damageable = null;
        if (instance.Target != null)
        {
            damageable = instance.Target.GetComponent<Damageable>();
        }

        // 各種効果適用
        ApplyEffectAmount(instance.Data.SkillType001, instance.Data, instance.Target, damageable);
        ApplyEffectAmount(instance.Data.SkillType002, instance.Data, instance.Target, damageable);
        ApplyEffectAmount(instance.Data.SkillType003, instance.Data, instance.Target, damageable);
        ApplyEffectAmount(instance.Data.SkillType004, instance.Data, instance.Target, damageable);

        // 攻撃スキルならヒットボックス起動
        if (IsAttackSkill(instance.Data))
        {
            if (hitDetector == null)
            {
                hitDetector = GetComponent<SkillHitDetector>();
                if (hitDetector == null)
                {
                    Debug.LogError("[SkillExecutor] SkillHitDetector が未設定です。");
                    return;
                }
            }

            // ★攻撃スキルは Target ではなく当たり判定から自動判定
            hitDetector.PerformHitDetection(instance, transform);

            // HitBox有効化（オプション）
            HitboxActiveSetter(instance);
        }
}

    public void HitboxActiveSetter(SkillInstance instance)
    {
        hitDetector.ActivateHitbox(0.2f); // ← 0.2秒間アクティブ
        hitDetector.PerformHitDetection(instance, transform);
    }
    private void ApplyEffectAmount(int skillType, SkillData skill, ParameterBase target, Damageable damageable)
    {
        if (skillType == 0) return; // スキル未設定行をスキップ

        switch ((SkillType)skillType)
        {
            case SkillType.Attack:
                lastEffectAmount = skill.EffectAmount001;
                Debug.Log($"[ApplyEffectAmount] 攻撃力 {lastEffectAmount}");
                break;

            case SkillType.Move:
                if (target != null)
                    target.MoveSpeed += skill.EffectAmount001;
                break;

            case SkillType.Heal:
                if (target != null)
                    target.Heal(skill.EffectAmount001);
                break;

            case SkillType.Buff:
                if (target != null)
                {
                    target.Attack += skill.EffectAmount001;
                    target.Defense += skill.EffectAmount002;
                    target.MoveSpeed += skill.EffectAmount003;
                }
                break;

            case SkillType.DoubleJump:
                if (playerController == null)
                    playerController = FindObjectOfType<PlayerController>();

        if (playerController != null)
        {
            // SkillDataに設定されたeffectDurationを使う
            float duration = skill.effectDuration > 0 ? skill.effectDuration : 5f;
            playerController.EnableTemporaryDoubleJump(duration);

            Debug.Log($"[SkillExecutor] 二段ジャンプ解禁！（{duration}秒間）");
        }
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

    private void ExecuteDoubleJump()
    {
        
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
