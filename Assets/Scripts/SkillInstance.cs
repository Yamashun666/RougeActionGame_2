using UnityEngine;

/// <summary>
/// スキルの1回の発動を表すインスタンス。
/// 実行中スキルの状態（発動者・対象・時間など）を保持。
/// </summary>
[System.Serializable]
public class SkillInstance
{
    public SkillData Data;           // スキルデータ本体
    public ParameterBase Caster;     // 発動者
    public ParameterBase Target;     // 対象
    public bool IsActive;            // 現在発動中か
    public float Timer;              // 発動経過時間
    public Transform SpawnPoint => Caster?.ModelRoot ?? null;


    public SkillInstance(SkillData data, ParameterBase caster, ParameterBase target)
    {
        Data = data;
        Caster = caster;
        Target = target;
        IsActive = true;
        Timer = 0f;
    }
}
