using UnityEngine;

/// <summary>
/// 敵の基礎データ（パラメータ、AI挙動、ドロップ設定など）をまとめた ScriptableObject。
/// ParameterBase がこれを参照して初期化する。
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyMaster", menuName = "Game/EnemyMaster", order = 1)]
public class EnemyMaster : ScriptableObject
{
    [Header("🧩 基本情報")]
    public string Name = "スライム";
    public Sprite EnemyIcon;
    [TextArea(1, 3)] public string Description;

    [Header("💀 ステータス（ParameterBase互換）")]
    public int MaxHP = 100;
    public int LimitOverHP = 0;
    public int Attack = 10;
    public int MagicPower = 0;
    public int Defense = 5;
    public int DOTDamageRate = 0;
    public int AttackSpeed = 1000;
    public int CTReduction = 0;
    public int MoveSpeed = 2;
    public int CriticalRate = 50;

    [Header("🧠 AI関連設定")]
    public bool IsAggressive = true;
    public float DetectRange = 5.0f;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 2.0f;
    public SkillData DefaultAttackSkill;

    [Header("🎁 スキルドロップ関連")]
    [Range(0, 1000)] public int SkillDropChance = 250; // ← 1000分率に変更（例：250=25%）
    public RarityDropTable RarityDropTable;
    public bool IsUniqueEnemy = false;

    [Header("💎 通常アイテムドロップ")]
    public GameObject[] DropPrefabs;
    [Range(0, 1000)] public int DropChance = 1000; // ← 同じく千分率で管理

    [Header("✨ 視覚効果")]
    public GameObject DeathEffectPrefab;
    public Color FlashColor = Color.white;

    [Header("🧱 その他タグ / 拡張用")]
    public bool IsBoss = false;
    public bool CanRespawn = false;
}

/// <summary>
/// レアリティ別ドロップテーブル（確率は1000分率）
/// </summary>
[System.Serializable]
public class RarityDropTable
{
    [Header("レアリティ別ドロップ確率（1000分率）")]
    [Range(0, 1000)] public int Common = 750;
    [Range(0, 1000)] public int Rare = 150;
    [Range(0, 1000)] public int Epic = 80;
    [Range(0, 1000)] public int Legendary = 20;

    /// <summary>
    /// 確率に応じてレアリティを抽選
    /// </summary>
    public int RollRarity()
    {
        int roll = Random.Range(0, 1000);
        if (roll < Common) return 1;
        if (roll < Common + Rare) return 2;
        if (roll < Common + Rare + Epic) return 3;
        return 4;
    }
}
