using UnityEngine;

/// <summary>
/// 敵の死亡時にアイテムをドロップする処理。
/// ParameterBase の OnDeath を購読して自動生成。
/// </summary>
public class EnemyDropHandler : MonoBehaviour
{
    [Header("参照")]
    public ParameterBase parameterBase; // 対象キャラのパラメータ
    public Transform dropPoint;         // ドロップ出現位置（空オブジェクト推奨）

    [Header("ドロップアイテム設定")]
    public GameObject[] dropPrefabs;    // ドロップ候補（Coin、Gemなど）
    [Range(0f, 1f)]
    public float dropChance = 1.0f;     // ドロップ確率（1.0=100%）

    private void Awake()
    {
        if (parameterBase == null)
            parameterBase = GetComponent<ParameterBase>();

        // OnDeath イベント購読
        if (parameterBase != null)
            parameterBase.OnDeath += TriggerDrop;
    }

    private void OnDestroy()
    {
        // イベント購読解除（メモリリーク防止）
        if (parameterBase != null)
            parameterBase.OnDeath -= TriggerDrop;
    }

    private void TriggerDrop()
    {
        if (dropPrefabs == null || dropPrefabs.Length == 0) return;
        if (Random.value > dropChance) return;

        // 出現位置決定
        Vector3 spawnPos = dropPoint != null ? dropPoint.position : transform.position;

        // ランダムに1つ選択して生成
        GameObject prefab = dropPrefabs[Random.Range(0, dropPrefabs.Length)];
        Instantiate(prefab, spawnPos, Quaternion.identity);

        Debug.Log($"💀 {parameterBase.Name} の死亡により {prefab.name} をドロップ！");
    }
}
