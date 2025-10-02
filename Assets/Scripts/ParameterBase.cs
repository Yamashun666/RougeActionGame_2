using UnityEngine;

[System.Serializable]
public class ParameterBase
{
    public string Name;         //　キャラ名
    public int Level;           //　キャラのレベル。レベルの数に応じてそのテーブルのステータスを渡す。
    public int MaxHP;           //　HPの最大値
    public int LimitOverHP;     // 上限超過HP
    public int CurrentHP;       //　HPの現在値
    public int DOTDamageRate;   //　継続ダメージの増減割合
    public int Attack;          //　攻撃力
    public int AttackSpeed;     //　通常攻撃の速度（どれだけ早く連打できるか）
    public int CTReduction;     //　スキルのキャストタイム加速
    public int MagicPower;      //　魔法攻撃力
    public int CriticalRate;    //　クリティカル率
    public int Defense;         //　防御力
    public int MoveSpeed;       //　移動速度



    // 共通の処理

public void TakeDamage(int damage)
{
    if (LimitOverHP > 0)
    {
        // 上限超過分（LimitOverHP）から先に削る
        int reduce = Mathf.Min(damage, LimitOverHP);
        LimitOverHP -= reduce;
        damage -= reduce;
    }

    if (damage > 0)
    {
        // 残りダメージは通常HPから
        CurrentHP = Mathf.Max(CurrentHP - damage, 0);
    }
}

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }
}

