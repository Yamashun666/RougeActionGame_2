using System;
using UnityEngine;

/// <summary>
/// キャラクターの共通パラメータを管理するクラス。
/// ステータス、被ダメージ、回復、バフ処理などを統一。
/// </summary>
[System.Serializable]
public class ParameterBase : MonoBehaviour
{
    [Header("基本情報")]
    public string Name;             // キャラ名
    public int Level;               // キャラレベル（ステータス参照用）

    [Header("体力系")]
    public int MaxHP;               // HP最大値
    public int CurrentHP;           // 現在のHP
    public int LimitOverHP;         // 上限超過HP（バリアやオーバーヒールに相当）

    [Header("攻撃・防御系")]
    public int Attack;              // 攻撃力
    public int MagicPower;          // 魔法攻撃力
    public int Defense;             // 防御力
    public int DOTDamageRate;       // 継続ダメージ倍率（1000分率）

    [Header("行動速度系")]
    public int AttackSpeed;         // 攻撃速度（1000=等倍、1500=1.5倍）
    public int CTReduction;         // クールタイム短縮（1000分率）
    public int MoveSpeed;           // 移動速度

    [Header("クリティカル系")]
    public int CriticalRate;        // クリティカル発生率（1000分率）

    [Header("位置・参照")]
    public Transform ModelRoot;     // 見た目やVFX発生位置の基準
    public Vector3 Position => ModelRoot ? ModelRoot.position : transform.position;
    public event Action OnDeath;
    public DamagePopupSpawner popupSpawner;
    public  EnemyMaster enemyMaster ;



    // ========================================
    //  基本ロジック
    // ========================================

    /// <summary>
    /// ダメージを受ける。
    /// LimitOverHP → CurrentHP の順で削られる。
    /// </summary>

    /// <summary>
    /// 回復処理。HP上限を超える場合はLimitOverHPに加算。
    /// </summary>
        private void Awake()
    {
        if (enemyMaster != null)
        {
            ApplyEnemyMasters(enemyMaster);
        }
    }

    public void ApplyEnemyMasters(EnemyMaster  master)
    {
        Name = master.Name;
        MaxHP = master.MaxHP;
        CurrentHP = master.MaxHP;
        LimitOverHP = master.LimitOverHP;
        Attack = master.Attack;
        MagicPower = master.MagicPower;
        Defense = master.Defense;
        DOTDamageRate = master.DOTDamageRate;
        AttackSpeed = master.AttackSpeed;
        CTReduction = master.CTReduction;
        MoveSpeed = master.MoveSpeed;
        CriticalRate = master.CriticalRate;

        Debug.Log($"[ParameterBase.ApplyEnemyMasters] {Name} のパラメータを EnemyMaster から初期化しました。");
    }
    public void Heal(int amount)
    {
        if (amount <= 0) return;

        int newHP = CurrentHP + amount;
        if (newHP > MaxHP)
        {
            LimitOverHP += newHP - MaxHP;
            CurrentHP = MaxHP;
        }
        else
        {
            CurrentHP = newHP;
        }
    }
// ParameterBase.cs
    public void TakeDamage(int damage)
    {
        // Damageableがいれば、そっちで処理を完結
        var damageable = GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        else
        {
            // Damageableがない場合は、最低限のHP処理だけやる
            CurrentHP = Mathf.Max(CurrentHP - damage, 0);
            if (CurrentHP <= 0)
            {
                Debug.Log($"{gameObject.name} は倒れた！（Damageable未設定）");
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// キャラクター死亡時の処理。
    /// </summary>

    /// <summary>
    /// ステータスの現在値を取得（ログ・デバッグ用）
    /// </summary>
    public void LogStatus()
    {
        Debug.Log($"[{Name}] HP:{CurrentHP}/{MaxHP} +{LimitOverHP}, ATK:{Attack}, DEF:{Defense}, SPD:{MoveSpeed}");
    }
}
