using System;
using UnityEngine;
using Game.SkillSystem;

public enum SkillType
{
    Attack = 1,
    Move = 2,
    Heal = 3,
    Buff = 4
}
[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/SkillData")]

public class SkillData : ScriptableObject
{
    public string SkillName;            // スキルの名称
    public string GroupCode;            // レベルすべてを包括したスキルのCODE
    public string LevelCode;            // GroupCodeをスキルレベルごとに分割したCODE
    public int Rarity;                  // このスキルのレアリティ。1=コモン 2=アンコモン 3 = レア 4 = エピック 5 = レジェンダリー
    public int SkillType001;            // どの効果を反映させるか。SkillTypeに定義。動きを重複させることも可能。
    public int SkillType002;            // どの効果を反映させるか。SkillTypeに定義。動きを重複させることも可能。
    public int SkillType003;            // どの効果を反映させるか。SkillTypeに定義。動きを重複させることも可能。
    public int SkillType004;            // どの効果を反映させるか。SkillTypeに定義。動きを重複させることも可能。
    public SkillType Type;              // ここにSkillTypeを代入（不要か？）
    public int EffectAmount001;         // どれだけの効果量を持つか？攻撃力に反映させるなら150でダメージ1.5倍など。どのパラメーターを作用させるかはSkillTypeの中で定義。
    public int EffectAmount002;         // どれだけの効果量を持つか？攻撃力に反映させるなら150でダメージ1.5倍など。どのパラメーターを作用させるかはSkillTypeの中で定義。
    public int EffectAmount003;         // どれだけの効果量を持つか？攻撃力に反映させるなら150でダメージ1.5倍など。どのパラメーターを作用させるかはSkillTypeの中で定義。
    public int CoolTime;                // スキルのCT短縮量。1000分立で表記。
    public int LevelUP_LevelCode;       // スキルレベルアップ時に渡すLevelCODE
    public string UseSkillSFX001;       // スキル使用時に再生するSFX。
    public float DelayUseSkillSFX001;   // UseSkillSFX001の再生遅延量。
    public string UseSkillSFX002;       // スキル使用時に再生するSFX。
    public float DelayUseSkillSFX002;   // UseSkillSFX002の再生遅延量。
    public string UseSkillVFX001;       // スキル使用時に再生するVFX。
    public float DelayUseSkillVFX001;   // UseSkillVFX001の再生遅延量。
    public string UseSkillVFX002;       // スキル使用時に再生するVFX。
    public float DelayUseSkillVFX002;   // UseSkillVFX002の再生遅延量。
    public string SkillEnhancementTable;// このスキルに渡すスキル成長テーブルの定義。Codeと同じ値を必ず渡すこと
    public string SkillIcon;            // このスキルのビジュアルアイコン。これのファイル名を検索し、UI上に表記。
    public string LevelUPSkillCode;     // レベルアップ時に渡すスキルのコード。Nullならレベルアップできる関数を呼ぶときにエラーを吐くようにしろ
    public HitShape HitShapeType; // 攻撃判定の形状
    
    [Header("特殊設定")]
    public bool IsUnique = false;

}