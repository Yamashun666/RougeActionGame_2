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
    private bool canInteract = false;

    private DroppedItem nearbyDrop; // 現在触れているDropを記録
    private bool isDragging = false;  // ドラッグ中フラグ


    private void Awake()
    {
        skillManager = GetComponent<SkillManager>();
        inventoryManager = GetComponent<InventoryManager>();
    }

    private void Update()
    {
        // 🟢 Fキー押下 → スキル取得 or ドラッグ開始
        if (canInteract && nearbyDrop != null && Input.GetKeyDown(KeyCode.F))
        {
            SkillData skill = nearbyDrop.skillData;
            if (skill == null)
            {
                Debug.LogError($"[PlayerInventory] SkillDataがnullです。対象={nearbyDrop.name}");
                return;
            }

            // ドラッグモード開始（今後UIでの操作用）
            StartDragMode(nearbyDrop);

            nearbyDrop = null;
            canInteract = false;
        }
            if (nearbyDrop != null)
        Debug.Log($"[Update] nearbyDrop = {nearbyDrop.name}, canInteract = {canInteract}");
        else
        /// Debug.Log("[Update] nearbyDrop = null");

        if (canInteract && nearbyDrop != null && Input.GetKeyDown(KeyCode.F))
        {
            SkillData skill = nearbyDrop.skillData;
        }

        // 🔴 Esc押下 → ドラッグモード解除
        if (isDragging && Input.GetKeyDown(KeyCode.Escape))
        {
            CancelDragMode();
        }

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

    private void StartDragMode(DroppedItem drop)
    {
        if (isDragging) return;
        if (drop == null)
        {
            Debug.LogError("[PlayerInventory] drop が null");
            return;
        }
        Debug.Log($"[PlayerInventory] StartDragMode 呼び出し: {drop.name}, skillData={(drop.skillData != null ? drop.skillData.SkillName : "null")}");
        // 🩹 応急処置：まだskillDataがnullなら再割り当て
        if (drop.skillData == null)
        {
            Debug.LogWarning($"[PlayerInventory] {drop.name} の skillData が null のため再割り当てを試行");
            var fallbackSkill = SkillDatabase.Instance.GetRandomSkillByRarity(1); // 例：仮に1(コモン)で拾う
            drop.AssignSkill(fallbackSkill);
        }

        if (drop.skillData == null)
        {
            Debug.LogError($"[PlayerInventory] drop.skillData が依然 null ({drop.name})");
            return;
        }

        isDragging = true;
        Debug.Log($"[PlayerInventory] ドラッグ開始 {drop.skillData.SkillName}");

        SkillOrbDragController.Instance.BeginDrag(drop.skillData, drop);
    }

        private void HighlightSkillOrb(DroppedItem drop, bool enable)
    {
        if (drop == null) return;
        var sr = drop.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = enable ? Color.yellow : Color.white;
        }
    }

    public void CancelDragMode()
    {
        if (!isDragging) return;

        isDragging = false;
        Debug.Log("[PlayerInventory] ドラッグモード解除 (Escキー)");
        HighlightSkillOrb(nearbyDrop, false);

        // TODO: 今後スキル説明UIやドラッグ用カーソルのリセット処理を追加
    }
    public void RegisterNearbyItem(DroppedItem drop)
    {
        if (drop == null)
        {
            Debug.LogError("[PlayerInventory] RegisterNearbyItem に null が渡された");
            return;
        }
        Debug.Log($"[PlayerInventory.RegisterNearbyItem] 呼び出された! drop={(drop != null ? drop.name : "null")}, this={gameObject.name}");
        nearbyDrop = drop;
        canInteract = true;

        Debug.Log($"[PlayerInventory] {drop.name} に接近。Fキーで拾えます。");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DroppedItem drop = other.GetComponent<DroppedItem>();
        if (drop == null) return;

        nearbyDrop = drop;
        canInteract = true;
        Debug.Log($"[PlayerInventory] {drop.name} に接近。Fキーで拾えます。");
    }

    public void UnregisterNearbyItem(DroppedItem item)
    {
        //if (nearbyDrop == item) nearbyDrop = null;
        // UI を消す
    }

    private void PickUp(DroppedItem item)
    {
        if (item == null || item.skillData == null)
        {
            Debug.LogWarning("[PlayerInventory] PickUp 失敗: item or skillData が null");
            return;
        }

        // 1) スキル取得（所持一覧 or UI への反映）
        var skillMgr = GetComponent<SkillManager>();
        if (skillMgr != null)
        {
            skillMgr.AddSkill(item.skillData);
        }

        // 2) スロットUIに自動割り当てしたいならここで呼ぶ
        // FindAnyObjectByType<SkillUIManager>()?.AssignToFirstEmptySlot(item.skillData);

        // 3) オーブを消す
        Destroy(item.gameObject);
        nearbyDrop = null;

        Debug.Log($"[PlayerInventory] [{item.skillData.SkillName}] を取得しました！");
    }
}

