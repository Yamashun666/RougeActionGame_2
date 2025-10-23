using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillOrbUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("スキル情報")]
    public SkillData skillData;              // このUIが表すスキル
    public Image iconImage;                  // スキルのアイコン画像

    private CanvasGroup canvasGroup;         // ドラッグ時に透明化する用
    private RectTransform rectTransform;     // UIの座標操作用
    private Transform originalParent;        // 元の親オブジェクト
    private Canvas parentCanvas;             // 移動時の基準Canvas
    private SkillSlotUI originSlot;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Canvas探索
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
            Debug.LogError("[SkillOrbUI] Canvasが見つかりません。");
    }

    private void Start()
    {
        if (skillData != null)
        {
            SetSkill(skillData);
        }
    }

    /// <summary>
    /// SkillDataをセットしてUIを更新
    /// </summary>
    public void SetSkill(SkillData data)
    {
        skillData = data;
        if (iconImage != null && data != null)
        {
            Sprite sprite = Resources.Load<Sprite>($"SkillIcons/{data.SkillIcon}");
            if (sprite != null)
            {
                iconImage.sprite = sprite;
                iconImage.enabled = true;
            }
            else
            {
                Debug.LogWarning($"[SkillOrbUI] アイコンが見つかりません: {data.SkillIcon}");
                iconImage.enabled = false;
            }
        }
    }

    public SkillData GetSkillData() => skillData;

    // ===============================
    //   ドラッグ処理
    // ===============================

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skillData == null) return;

        originalParent = transform.parent;
        transform.SetParent(parentCanvas.transform); // Canvas直下に移動（最前面でドラッグ）
        canvasGroup.alpha = 0.6f;                    // 半透明化
        canvasGroup.blocksRaycasts = false;          // ドロップ先のUIを検知可能に
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillData == null) return;

        // マウスに追従
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (skillData == null) return;

        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero; // 元位置に戻す
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
        public void SetOriginSlot(SkillSlotUI slot)
    {
        originSlot = slot;
    }

    public SkillSlotUI GetOriginSlot()
    {
        return originSlot;
    }
}
