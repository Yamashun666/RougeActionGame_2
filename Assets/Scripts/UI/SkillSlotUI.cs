using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI構成")]
    public Image highlightFrame;
    public int slotIndex;
    public SkillData assignedSkill{ get; private set; }
    public DroppedItem assignedDroppedItem;

    private Canvas canvas; // 親Canvasを取得してUI座標を変換するため
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private SkillOrbDragController skillOrbDragController;

    private Vector2 originalPosition;
    private bool isDragging = false;
    private PlayerController playerController;
    public SkillUIManager skillUIManager;
    [Header("UI")]
    public TMPro.TextMeshProUGUI skillNameText;
    public UnityEngine.UI.Image cooldownMask;
    public Image iconImage;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        playerController = FindObjectOfType<PlayerController>();
    }


    // =======================================
    // 💠 スキル登録・削除
    // =======================================
    public void SetSkill(SkillData skill, DroppedItem dropItem, Sprite overrideIcon)
    {
        assignedSkill = skill;
        assignedDroppedItem = dropItem;

        if (iconImage == null)
        {
            Debug.LogWarning($"[SkillSlotUI] スロット {slotIndex} にIconが見つかりません。");
            return;
        }

        // 優先順：overrideIcon > dropItem.defaultIcon > SkillData.SkillIcon
        Sprite iconToUse = overrideIcon ?? dropItem?.defaultIcon;

        if (iconToUse == null && !string.IsNullOrEmpty(skill?.SkillIcon))
        {
            // SkillData.SkillIcon に "SkillIcons/DoubleJump" のようなパスが入っている想定
            iconToUse = Resources.Load<Sprite>($"SkillIcons/{skill.SkillIcon}");
        }

        if (iconToUse != null)
        {
            iconImage.sprite = iconToUse;
            iconImage.color = Color.white;
            Debug.Log($"[SkillSlotUI] スロット {slotIndex} に {skill?.SkillName ?? "null"} のアイコンを設定しました。");
        }
        else
        {
            Debug.LogWarning($"[SkillSlotUI] スロット {slotIndex} に表示可能なアイコンがありません。");
        }
        if (playerController != null)
        {
            if (skill.SkillType001 == 7 || skill.SkillType002 == 7 ||
                skill.SkillType003 == 7 || skill.SkillType004 == 7)
            {
                playerController.hasJetBoost = true;
                playerController.jetBoostSkill = skill;  // ← これ大事
                Debug.Log("[SkillSlotUI] JetBoostスキル検出 → isJetBoostActive = TRUE");
            }
        }

    }
    public void ReplaceSkill(SkillData newSkill)
    {
        assignedSkill = newSkill;
        RefreshUI();
    }
    public void ClearSlot()
    {
        assignedSkill = null;
        assignedDroppedItem = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.color = new Color(1, 1, 1, 0);
        }
    }
    private void RefreshUI()
    {
        if (assignedSkill == null)
        {
            iconImage.enabled = false;
            skillNameText.text = "";
            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = Resources.Load<Sprite>(assignedSkill.SkillIcon);
        skillNameText.text = assignedSkill.SkillName;

        // クールタイムUI初期化などもここ
    }
    // alias互換用
    public void ClearSkill() => ClearSlot();

    // =======================================
    // 🎯 ドロップ時（他のオブジェクトやスキルOrbから）
    // =======================================
    public void OnDrop(PointerEventData eventData)
    {
        if (!SkillOrbDragController.Instance.IsDragging) return;

        SkillData draggedSkill = SkillOrbDragController.Instance.GetDraggedSkill();
        DroppedItem originDrop = SkillOrbDragController.Instance.GetOriginDrop();

        if (draggedSkill != null)
        {
            SetSkill(draggedSkill, originDrop, skillOrbDragController.cachedIcon);
            SkillOrbDragController.Instance.EndDrag();
            HighlightSlot();
            Debug.Log($"[SkillSlotUI] Slot {slotIndex} に {draggedSkill.SkillName} を登録しました。");
        }
    }

    // =======================================
    // 💡 ドラッグ＆ドロップ（スロット間の移動）
    // =======================================
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (assignedSkill == null) return;

        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        SkillOrbDragController.Instance.BeginDragFromSlot(this);
        Debug.Log($"[SkillSlotUI] Slot {slotIndex} からドラッグ開始");

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 pos
        );
        rectTransform.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        rectTransform.anchoredPosition = originalPosition;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 地面にドロップされた場合
        if (!eventData.pointerCurrentRaycast.gameObject)
        {
            DropSkillToField();
        }

        SkillOrbDragController.Instance.EndDrag();
    }

    // =======================================
    // ✨ 地面へのドロップ処理
    // =======================================
    private void DropSkillToField()
    {
        if (assignedSkill == null) return;

        // Prefabロード
        var orbPrefab = Resources.Load<GameObject>("Prefabs/SkillOrb");
        if (orbPrefab == null)
        {
            Debug.LogError("[SkillSlotUI] SkillOrb prefab が Resources/Prefabs に存在しません。");
            return;
        }

        // プレイヤー付近に出現
        var player = GameObject.FindGameObjectWithTag("Player");
        Vector3 dropPos = player ? player.transform.position + Vector3.right * 1.5f : Vector3.zero;
        var orb = Object.Instantiate(orbPrefab, dropPos, Quaternion.identity);

        var dropItem = orb.GetComponent<DroppedItem>();
        if (dropItem != null)
        {
            dropItem.AssignSkill(assignedSkill);
        }

        Debug.Log($"[SkillSlotUI] スキル [{assignedSkill.SkillName}] を地面にドロップしました。");
        ClearSlot();
    }

    // =======================================
    // 🌟 視覚演出
    // =======================================
    private void HighlightSlot()
    {
        if (highlightFrame != null)
        {
            highlightFrame.gameObject.SetActive(true);
            CancelInvoke(nameof(ClearHighlight));
            Invoke(nameof(ClearHighlight), 0.5f);
        }
    }

    private void ClearHighlight()
    {
        if (highlightFrame != null)
            highlightFrame.gameObject.SetActive(false);
    }

    internal void SetSkill(SkillData dummy, DroppedItem dummy2, SkillOrbDragController dummy3)
    {
        throw new System.NotImplementedException();
    }

}
