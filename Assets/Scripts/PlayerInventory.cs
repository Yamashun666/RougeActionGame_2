using UnityEngine;
using System;
[RequireComponent(typeof(SkillManager))]

public class PlayerInventory : MonoBehaviour
{
    public int coinCount = 0;
    public event Action<int> OnCoinChanged; // UI更新イベント
    private SkillManager skillManager;
    private InventoryManager inventoryManager;
    public SkillData skillData;
    private void Awake()
    {
        skillManager = GetComponent<SkillManager>();
        inventoryManager = GetComponent<InventoryManager>();
    }
    public void AddItem(string itemName, int value)
    {
        if (itemName == "Coin")
        {
            coinCount += value;
            OnCoinChanged?.Invoke(coinCount); // UI通知

        }

        Debug.Log($"🧍 コイン獲得！ 合計: {coinCount}");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        DroppedItem drop = other.GetComponent<DroppedItem>();
        if (drop != null)
        {
            SkillData skill = drop.skillData;
            if (skill != null)
            {
                Debug.LogWarning("Skill Founded");
                skillManager.AddSkill(skill);
                Debug.Log($"🧠 スキル [{skill.SkillName}] を取得しました！");
            }
            else
            {
                Debug.LogError("[PlayerInventory.OnTriggerEnter2D] SkillData Not Found");
            }
        }
        if (drop == null)
        {
            // Debug.Log($"[{name}] {other.name} は DroppedItem を持たないためスルー");
            return;
        }
        Debug.Log($"[{name}] {drop.name} を拾いました！");
        CollectDroppedItem(drop);

    }
    private void CollectDroppedItem(DroppedItem droppedItem)
{
    if (droppedItem == null)
    {
        Debug.LogWarning("[PlayerInventory] DroppedItem が null です。");
        return;
    }

    // ① インベントリに追加
    // 　もし inventoryManager などの管理クラスがあればここで呼ぶ
    if (inventoryManager != null)
    {
        inventoryManager.AddItem(droppedItem);
    }

    // ② スキルドロップの場合：SkillManager に登録
    if (droppedItem.skillData != null)
    {
        Debug.Log($"[PlayerInventory] スキル {droppedItem.skillData.SkillName} を獲得！");
        // SkillManager.Instance.AddSkill(droppedItem.skillData);
    }

    // ③ サウンド or エフェクト再生
    // AudioManager.Play("ItemGet"); // ←任意

    // ④ ゲーム上のアイテムを削除
    Destroy(droppedItem.gameObject);
}
}

