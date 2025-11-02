using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 複数の EnemyGambitSO をまとめて1体の敵AIが参照する
/// </summary>
[CreateAssetMenu(fileName = "EnemyGambitSet_", menuName = "Enemy/GambitSet", order = 2)]
public class EnemyGambitSet : ScriptableObject
{
    [Header("ガンビットリスト")]
    public List<EnemyGambitSO> gambits = new List<EnemyGambitSO>();
}
