using UnityEngine;

namespace Game.SkillSystem
{
    /// <summary>
    /// スキルの攻撃判定形状
    /// </summary>
    public enum HitShape
    {
        Box,        // 四角形の判定
        Capsule,    // カプセル型（縦長）
        Ray         // レイ判定（一直線）
    }
}
