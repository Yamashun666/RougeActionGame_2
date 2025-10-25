using UnityEngine;

/// <summary>
/// 敵の死亡時にスキル・アイテムをドロップする処理。
/// ParameterBase が参照している EnemyMaster の設定をもとに判定する。
/// </summary>
[RequireComponent(typeof(ParameterBase))]
public class EnemyDropHandler : MonoBehaviour
{
    [Header("参照")]
    public ParameterBase parameterBase;     // 敵のパラメータ（ここから EnemyMaster を参照）

    private void Awake()
    {
        if (parameterBase == null)
            parameterBase = GetComponent<ParameterBase>();

        if (parameterBase != null)
            parameterBase.OnDeath += TriggerDrop;
    }

    private void OnDestroy()
    {
        if (parameterBase != null)
            parameterBase.OnDeath -= TriggerDrop;
    }

    /// <summary>
    /// 死亡時ドロップ処理のエントリーポイント
    /// </summary>
    private void TriggerDrop()
    {
        if (parameterBase.enemyMaster == null)
        {
            Debug.LogWarning($"[EnemyDropHandler] {gameObject.name} に EnemyMaster が設定されていません。");
            return;
        }

        var master = parameterBase.enemyMaster;

        TryDropItem(master);
        TryDropSkill(master);
    }

    // ======================================
    // 💎 通常アイテムドロップ処理
    // ======================================
    private void TryDropItem(EnemyMaster master)
    {
        if (master.DropPrefabs == null || master.DropPrefabs.Length == 0) return;

        int roll = Random.Range(0, 1000);
        if (roll >= master.DropChance)
        {
            Debug.Log($"[EnemyDropHandler] {master.Name} はアイテムをドロップしなかった。(roll:{roll})");
            return;
        }

        GameObject prefab = master.DropPrefabs[Random.Range(0, master.DropPrefabs.Length)];
        Instantiate(prefab, transform.position, Quaternion.identity);
        Debug.Log($"💎 {master.Name} が {prefab.name} をドロップ！");
    }

    // ======================================
    // 🎁 スキルドロップ処理
    // ======================================
    private void TryDropSkill(EnemyMaster master)
    {
        // 1️⃣ スキルドロップ確率判定
        int roll = Random.Range(0, 1000);
        if (roll >= master.SkillDropChance)
        {
            Debug.Log($"[EnemyDropHandler] {master.Name} はスキルをドロップしなかった。(roll:{roll})");
            return;
        }

        // 2️⃣ ユニーク敵はスキップ
        if (master.IsUniqueEnemy)
        {
            Debug.Log($"[EnemyDropHandler] ユニーク敵 {master.Name} はスキルをドロップしません。");
            return;
        }

        // 3️⃣ レアリティ抽選（1000分率対応）
        int rolledRarity = master.RarityDropTable.RollRarity();

        // 4️⃣ SkillDatabase から該当レアリティのスキルを取得
        SkillData skill = SkillDatabase.Instance.GetRandomSkillByRarity(rolledRarity, excludeUnique: true);
        if (skill == null)
        {
            Debug.Log($"[EnemyDropHandler] レアリティ({rolledRarity})のスキルが見つかりません。");
            return;
        }

        // 5️⃣ SkillOrb生成
        var orbPrefab = Resources.Load<GameObject>("Prefabs/SkillOrb");
        if (orbPrefab == null)
        {
            Debug.LogError("[EnemyDropHandler] SkillOrb prefab が Resources/Prefabs に存在しません。");
            return;
        }

        Vector3 spawnPos = transform.position;
        GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);

        var dropItem = orb.GetComponent<DroppedItem>();
        if (dropItem != null)
        {
            dropItem.AssignSkill(skill);
        }

        Debug.Log($"✨ {master.Name} がスキル [{skill.SkillName}] (Rarity={rolledRarity}) をドロップ！ roll={roll}");
    }
}
