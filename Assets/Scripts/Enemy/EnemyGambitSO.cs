using UnityEngine;

public enum ConditionFlag
{
    HPPercent = 1,   // HPが◯％以下
    Distance = 2,    // プレイヤーとの距離が◯m以内
    AllyDeadCount = 3, // 味方の死亡数が◯体以上
    Always = 99      // 常にTrue（デフォルト行動などに使う）
}

public enum TargetType
{
    Player = 1,
    Self = 2,
    Ally = 3,        // 味方
    Enemy = 4,       // 敵
}

/// <summary>
/// 敵AIが条件付きでスキルを発動するための単体データ。
/// </summary>
[CreateAssetMenu(fileName = "EnemyGambitSO_", menuName = "Enemy/GambitSO", order = 1)]
public class EnemyGambitSO : ScriptableObject
{
    [Header("識別情報")]
    public string gambitName;          // 例：HP70%以下で回復
    public int priority = 0;           // 数値が大きいほど優先度が高い

    [Header("条件設定")]
    public ConditionFlag flagNum;      // どの条件を使うか
    public TargetType targetNum;       // 対象は誰か（プレイヤー or 自分など）
    public float targetPoint = 0f;     // 条件閾値（HP%や距離など）

    [Header("発動アクション")]
    public string callActionCode;      // 呼び出すスキルコード（SkillDatabase参照）

    [Header("デバッグ")]
    [TextArea(2, 4)]
    public string note;
}
