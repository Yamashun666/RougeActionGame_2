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
    public SkillSlotUI skillSlotUI;
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
        if (drop == null) return;

        SkillData skill = drop.GetAssignedSkill();
        if (skill == null)
        {
            Debug.LogError("[PlayerInventory] SkillDataがnullです。");
            return;
        }

        // スキルをSkillManagerに登録
        skillManager.AddSkill(skill);
        Debug.Log($"🧠 スキル [{skill.SkillName}] を取得しました！");

        // UIに反映
        if (SkillUIManager.Instance != null)
        {
            SkillUIManager.Instance.CreateSkillOrbUI(skill);
        }

        Destroy(other.gameObject); // 地面上のオーブを削除
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

