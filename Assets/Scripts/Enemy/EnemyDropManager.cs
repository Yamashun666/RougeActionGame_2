using UnityEngine;
using Game.SkillSystem;

public class EnemyDropManager : MonoBehaviour
{
    [Header("スキルドロップ確率（1000分率）")]
    public int CommonDropRate = 600;
    public int UnCommonDropRate = 250;
    public int RareDropRate = 100;
    public int EpicDropRate = 40;
    public int LegendaryDropRate = 10;

    [Header("ドロップ位置オフセット")]
    public Vector2 dropOffset = new Vector2(0, 1f);

    private SkillDatabase skillDB;

    private void Start()
    {
        skillDB = SkillDatabase.Instance;
    }

    public void TryDrop()
    {
        int roll = Random.Range(0, 1000);
        int rarity = GetRarityByRoll(roll);
        Debug.Log($"[EnemyDrop] 抽選結果: {roll} → Rarity={rarity}");

        SkillData skill = skillDB.GetRandomSkillByRarity(rarity);
        if (skill == null)
        {
            Debug.LogWarning($"[EnemyDrop] Rarity={rarity} のスキルが存在しません。");
            return;
        }

        // ✅ スキルに対応したPrefabを使用
        if (skill.DropPrefab == null)
        {
            Debug.LogWarning($"[EnemyDrop] {skill.SkillName} に DropPrefab が設定されていません。");
            return;
        }

        Vector3 dropPos = transform.position + (Vector3)dropOffset;
        GameObject orb = Instantiate(skill.DropPrefab, dropPos, Quaternion.identity, null);
        Debug.Log($"[EnemyDrop] Orb生成確認: {orb.name}");

        // DroppedItemがついていれば初期化
        var dropComp = orb.GetComponent<DroppedItem>();
        if (dropComp != null) dropComp.Initialize(skill);

        Debug.Log($"[EnemyDrop] スキル [{skill.SkillName}] のOrbを生成しました。({rarity})");
    }

    private int GetRarityByRoll(int roll)
    {
        if (roll < LegendaryDropRate)
            return 5;
        else if (roll < LegendaryDropRate + EpicDropRate)
            return 4;
        else if (roll < LegendaryDropRate + EpicDropRate + RareDropRate)
            return 3;
        else if (roll < LegendaryDropRate + EpicDropRate + RareDropRate + UnCommonDropRate)
            return 2;
        else
            return 1;
    }
}
