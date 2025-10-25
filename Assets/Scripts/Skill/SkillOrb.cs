using UnityEngine;
using UnityEngine.EventSystems;

public class SkillOrbUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("参照")]
    public DroppedItem linkedDroppedItem; // 元データ
    public SkillSlotUI originSlot;        // ドラッグ元スロットを記録

    private CanvasGroup canvasGroup;
    private Transform originalParent;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public SkillData GetSkillData() => linkedDroppedItem?.GetAssignedSkill();
    public DroppedItem GetDroppedItem() => linkedDroppedItem;
    public SkillSlotUI GetOriginSlot() => originSlot;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originSlot = GetComponentInParent<SkillSlotUI>();
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        canvasGroup.blocksRaycasts = true;
    }
}
