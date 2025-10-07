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


    // ========================================
    //  基本ロジック
    // ========================================

    /// <summary>
    /// ダメージを受ける。
    /// LimitOverHP → CurrentHP の順で削られる。
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        // 上限超過HPを優先して削る
        if (LimitOverHP > 0)
        {
            int reduce = Mathf.Min(damage, LimitOverHP);
            LimitOverHP -= reduce;
            damage -= reduce;
        }

        // 残りダメージは通常HPへ
        if (damage > 0)
        {
            CurrentHP = Mathf.Max(CurrentHP - damage, 0);
            if (CurrentHP <= 0)
            {
                OnDeath?.Invoke();
            }
        }

    }

    /// <summary>
    /// 回復処理。HP上限を超える場合はLimitOverHPに加算。
    /// </summary>
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
