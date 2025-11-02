using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.SkillSystem;

public class SkillExecutor : MonoBehaviour
{
    public int lastEffectAmount;
    private List<SkillInstance> activeSkills = new List<SkillInstance>();
    public ParameterBase parameterBase;
    [Header("SkillHitDetector")]
    public SkillHitDetector hitDetector;

    [Header("エフェクト / サウンド")]
    public AudioSource audioSource;
    public Transform effectOrigin;
    public PlayerController playerController;
    public SkillData skillData;
    MagicProjectile magicProjectile;
    Damageable damageable;

    private void Start()
    {
        hitDetector = GetComponent<SkillHitDetector>();
        Debug.Log($"[SkillExecutor] hitDetector取得確認: {(hitDetector == null ? "null" : hitDetector.name)}");
        playerController = GetComponent<PlayerController>();
    }
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


    // =============================
    //  スキル発動処理
    // =============================
    public void ExecuteSkill(SkillData skill, ParameterBase caster, ParameterBase target)
    {
        Debug.Log("ExecuteSkill()Called");
        if (skill == null || caster == null)
        {
            Debug.LogWarning("[SkillExecutor] 無効なスキルまたはキャスターが指定されました。");
            return;
        }
        Debug.Log($"[ExecuteSkill] {skill.SkillName} type(int)={skill.SkillType001} enum={(SkillType)skill.SkillType001}");
        SkillInstance instance = new SkillInstance(skill, caster, target);
        activeSkills.Add(instance);
        ApplySkillEffect(instance);
    }

    // =============================
    //  効果適用処理
    // =============================
    private void ApplySkillEffect(SkillInstance instance)
    {
        Debug.Log("ApplySkillEffect Called");
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
        ApplyEffectAmount(instance.Data.SkillType001, instance.Data, instance.Target, damageable, instance);
        ApplyEffectAmount(instance.Data.SkillType002, instance.Data, instance.Target, damageable, instance);
        ApplyEffectAmount(instance.Data.SkillType003, instance.Data, instance.Target, damageable, instance);
        ApplyEffectAmount(instance.Data.SkillType004, instance.Data, instance.Target, damageable, instance);

        // 攻撃スキルならヒットボックス起動
        if (IsAttackSkill(instance.Data))
        {
            Debug.Log("[SkillExecutor.ApplySkillEffect] IsAttackSkillが有効です。GenerateHitbox(instance)を起動します。");
            GenerateHitbox(instance);
        }
    }
    public void GenerateHitbox(SkillInstance instance)
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
    // SkillExecutor.cs 内に追加
    public void OnHitEnemy(ParameterBase target)
    {
        if (target == null)
        {
            Debug.LogWarning("[SkillExecutor.OnHitEnemy] targetがnullです。");
            return;
        }

        // Damageableコンポーネント経由で処理
        var damageable = target.GetComponent<Damageable>();
        if (damageable == null)
        {
            Debug.LogWarning("[SkillExecutor.OnHitEnemy] Damageableが見つかりません。");
            return;
        }

        int damage = Mathf.Max(1, lastEffectAmount - target.Defense);
        damageable.TakeDamage(damage);

        Debug.Log($"[OnHitEnemy] {target.name} に {damage} ダメージを与えた！");
    }

    private void ApplyEffectAmount(int skillType, SkillData skill, ParameterBase target, Damageable damageable,SkillInstance instance)
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
                Debug.Log("[SkillExecutor.ApplyEffectAmount]Called DoubleJump");
                if (playerController == null)
                    playerController = FindObjectOfType<PlayerController>();
                ExecuteDoubleJump(skillData,parameterBase);
                break;

            case SkillType.StepBackAttack:
                lastEffectAmount = skill.EffectAmount001;
                ExecuteStepBackAttack(skill, target, instance);
                break;

            case SkillType.RangedMagic:
                Debug.Log("[SkillExecutor.ApplyEffectAmount]Called RangedMagic");
                ExecuteProjectile(skill, target);
                break;

            case SkillType.DrainAttack:
                // 攻撃スキルとして処理される（ヒット判定も出る）
                // 自分への回復処理だけここで行う

                if (instance == null || instance.Caster == null)
                {
                    Debug.LogWarning("[DrainAttack] instanceまたはcasterがnullです。");
                    break;
                }

                var caster = instance.Caster;

                float randomFactor = UnityEngine.Random.Range(0.97f, 1.03f);
                float ratio = skill.EffectAmount002 / 100f;  // 吸収割合（例：50なら50%）
                int baseValue = skill.EffectAmount001;       // ダメージ量

                int healAmount = Mathf.RoundToInt(baseValue * ratio * randomFactor);

                caster.Heal(healAmount);
                Debug.Log($"[DrainAttack] {caster.name} が {healAmount} 回復しました。");
                break;

        }
    }
    private void ExecuteStepBackAttack(SkillData skill, ParameterBase caster, SkillInstance instance)
    {
        var player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        Debug.Log("[SkillExecutor] ステップバックアタック発動");

        // 1️⃣ ステップバック
        player.PerformStepBack(skill.StepBackDistance, skill.StepBackSpeed);

        // 2️⃣ 攻撃判定（ヒットボックス or Raycast）
        if (hitDetector == null)
            hitDetector = GetComponent<SkillHitDetector>();

        hitDetector.PerformHitDetection(new SkillInstance(skill, caster, null), player.transform);

        // 3️⃣ 演出呼び出し
        SkillEffectPlayer.Instance?.PlaySkillEffects(skill, player.transform);
        GenerateHitbox(instance);
    }
        public void ExecuteDoubleJump(SkillData skill, ParameterBase caster)
    {
        Debug.Log("ExecuteDoubleJump Called");
        var player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        player.EnableTemporaryDoubleJump();
        Debug.Log("[SkillExecutor] 二段ジャンプスキルを発動！");

        // 🟢 ここでエフェクト呼び出し！
        if (SkillEffectPlayer.Instance != null)
        {
            SkillEffectPlayer.Instance.PlaySkillEffects(skill, player.transform);
        }
        else
        {
            Debug.LogWarning("[SkillExecutor] SkillEffectPlayer.Instance が存在しません。シーンに配置されていますか？");
        }
    }
    private void ExecuteProjectile(SkillData skill, ParameterBase caster)
    {
        Debug.Log("[ExecuteProjectile] 呼ばれた");

        if (skill == null)
        {
            Debug.LogError("[ExecuteProjectile] skill が null");
            return;
        }

        if (skill.ProjectilePrefab == null)
        {
            Debug.LogError("[ExecuteProjectile] ProjectilePrefab が null");
            return;
        }

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("[ExecuteProjectile] PlayerController が null");
            return;
        }

        Debug.Log($"[ExecuteProjectile] player 取得成功: {player.name}");

        if (player.magicOrigin == null)
        {
            Debug.LogError("[ExecuteProjectile] magicOrigin が null");
            return;
        }

        GameObject projectile = Instantiate(skill.ProjectilePrefab, player.magicOrigin.position, Quaternion.identity);
        Debug.Log($"[ExecuteProjectile] Projectile生成: {projectile.name}");

        var proj = projectile.GetComponent<MagicProjectile>();
        if (proj == null)
        {
            Debug.LogError("[ExecuteProjectile] MagicProjectile スクリプトがPrefabにアタッチされていません！");
            return;
        }

        proj.Initialize(skill, caster, Mathf.Sign(player.transform.localScale.x));
        Debug.Log("[ExecuteProjectile] Initialize完了");
    }
    private bool IsAttackSkill(SkillData skill)
    {
        return skill.SkillType001 == (int)SkillType.Attack ||
               skill.SkillType002 == (int)SkillType.Attack ||
               skill.SkillType003 == (int)SkillType.Attack ||
               skill.SkillType004 == (int)SkillType.Attack ||
               skill.SkillType001 == (int)SkillType.DrainAttack ||
               skill.SkillType002 == (int)SkillType.DrainAttack ||
               skill.SkillType003 == (int)SkillType.DrainAttack ||
               skill.SkillType004 == (int)SkillType.DrainAttack;
    }
    public void HitboxActiveSetter(SkillInstance instance)
    {
        hitDetector.ActivateHitbox(0.2f); // ← 0.2秒間アクティブ
        hitDetector.PerformHitDetection(instance, transform);
    }
}
