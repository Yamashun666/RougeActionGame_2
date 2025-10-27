using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillOrbDragController : MonoBehaviour
{
    public static SkillOrbDragController Instance;

    [Header("ドラッグ中アイコンUI")]
    public Canvas dragCanvas;
    public Image dragIconImage;

    [HideInInspector]private bool isDragging = false;
    private SkillData currentSkill;
    private DroppedItem currentDrop;
    private SkillData draggedSkill;                // 現在ドラッグ中のスキル
    private DroppedItem originDroppedItem;         // 元のドロップオブジェクト（スロットまたはフィールド）
    public Sprite cachedIcon; // ★追加：破棄前にアイコンだけキャッシュ
    private static List<RaycastResult> reusableResults = new List<RaycastResult>(10);
    private bool isEndingDrag = false;





    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (dragCanvas == null)
            Debug.LogError("[SkillOrbDragController] dragCanvas が未設定です。");

        if (dragIconImage != null)
            dragIconImage.enabled = false;
    }

    private void Update()
    {
        if (!isDragging || isEndingDrag) return;

        // 🖱️ マウス追従
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragCanvas.transform as RectTransform,
            Input.mousePosition,
            dragCanvas.worldCamera,
            out mousePos
        );
        dragIconImage.rectTransform.localPosition = mousePos;

        // 🖱️ 左クリックアップでドロップ判定
        if (Input.GetMouseButtonUp(0))
        {
            isEndingDrag = true; // ← 重複防止
            EndDrag();
            isEndingDrag = false;
        }

        // 🔴 Escキーでキャンセル
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelDrag();
        }
    }
    public void BeginDragFromSlot(SkillSlotUI originSlot)
    {
        if (originSlot == null || originSlot.assignedSkill == null)
        {
            Debug.LogWarning("[SkillOrbDragController] BeginDragFromSlot: スキルなし or null slot");
            return;
        }

        isDragging = true;
        draggedSkill = originSlot.assignedSkill;
        originDroppedItem = originSlot.assignedDroppedItem;

        // ドラッグアイコン設定
        if (dragIconImage != null)
        {
            dragIconImage.sprite = originSlot.assignedDroppedItem?.defaultIcon;
            dragIconImage.enabled = true;
        }

        Debug.Log($"[SkillOrbDragController] スロットからスキル [{draggedSkill.SkillName}] のドラッグを開始しました。");
    }


    // ===========================================
    // ドラッグ開始（拾ったオーブなどから）
    // ===========================================
    public void BeginDrag(SkillData skill, DroppedItem drop)
    {
        if (skill == null || drop == null)
        {
            Debug.LogError("[SkillOrbDragController] BeginDrag失敗：skill or drop が null");
            return;
        }

        isDragging = true;
        draggedSkill = skill;
        originDroppedItem = drop;
        cachedIcon = drop.defaultIcon;
        Debug.Log($"[SkillOrbDragController] draggedSkill={draggedSkill?.SkillName ?? "null"}");


        if (dragIconImage != null)
        {
            dragIconImage.sprite = cachedIcon;
            dragIconImage.enabled = true;
            dragIconImage.gameObject.SetActive(true);
        }

        Debug.Log($"[SkillOrbDragController] BeginDrag: {skill.SkillName} 開始 (icon={cachedIcon != null})");
    }

    /// <summary>
    /// ドラッグ終了（UIスロット or キャンセル時）
    /// </summary>
    public void EndDrag()
    {
        if (!isDragging)
        {
            // すでに終了している
            return;
        }

        if (draggedSkill == null || originDroppedItem == null)
        {
            Debug.LogError($"[SkillOrbDragController] EndDrag失敗 skill={draggedSkill?.SkillName ?? "null"} drop={originDroppedItem?.name ?? "null"}");
            StopDragVisuals();
            return;
        }

        // イベント結果取得
        PointerEventData pointer = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        reusableResults.Clear();
        EventSystem.current.RaycastAll(pointer, reusableResults);

        bool registered = false;
        foreach (var result in reusableResults)
        {
            SkillSlotUI slot = result.gameObject.GetComponentInParent<SkillSlotUI>();
            if (slot != null)
            {
                slot.SetSkill(draggedSkill, originDroppedItem, cachedIcon);
                Debug.Log($"[SkillOrbDragController] スロット {slot.slotIndex} に {draggedSkill.SkillName} を登録しました。");
                registered = true;
                break;
            }
        }

        if (!registered)
        {
            Debug.Log("[SkillOrbDragController] 有効なUIスロットが見つかりませんでした。");
        }

        // 成功しても失敗しても一旦リセット
        if (originDroppedItem != null)
        {
            Debug.Log($"[SkillOrbDragController] {originDroppedItem.name} を削除");
            Destroy(originDroppedItem.gameObject);
        }

        StopDragVisuals();
    }


    /// <summary>
    /// Esc等によるキャンセル
    /// </summary>
    public void CancelDrag()
    {
        if (!isDragging) return;
        Debug.Log("[SkillOrbDragController] ドラッグキャンセル");
        StopDragVisuals();
    }
    private void StopDragVisuals()
    {
        isDragging = false;

        if (dragIconImage != null)
        {
            dragIconImage.enabled = false;
            dragIconImage.gameObject.SetActive(false);
        }

        draggedSkill = null;
        originDroppedItem = null;
        cachedIcon = null;
    }

    public bool IsDragging => isDragging;
    public SkillData GetDraggedSkill() => draggedSkill;
    public DroppedItem GetOriginDrop() => originDroppedItem;
}
