using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlotUI : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public Image iconImage;
    public SkillData assignedSkill;

    public void SetSkill(SkillData skill)
    {
        assignedSkill = skill;

        if (skill != null)
        {
            // 🔹 string → Sprite に変換して読み込み
            Sprite loadedSprite = Resources.Load<Sprite>($"Icons/{skill.SkillIcon}");

            if (loadedSprite != null)
            {
                iconImage.sprite = loadedSprite;
                iconImage.enabled = true;
            }
            else
            {
                Debug.LogWarning($"[SkillSlotUI] スプライトが見つかりません: {skill.SkillIcon}");
                iconImage.enabled = false;
            }
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public void ClearSkill()
    {
        assignedSkill = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedOrb = eventData.pointerDrag?.GetComponent<SkillOrbUI>();
        if (draggedOrb == null) return;

        SkillData droppedSkill = draggedOrb.GetSkillData();
        SkillSlotUI fromSlot = draggedOrb.GetOriginSlot();

        if (fromSlot == this) return;

        // 🔁 スロット入れ替え処理
        SkillData temp = assignedSkill;
        SetSkill(droppedSkill);
        if (fromSlot != null) fromSlot.SetSkill(temp);

        Debug.Log($"[SkillSlotUI] {slotIndex} ←→ {fromSlot?.slotIndex} 入れ替え完了");
    }
}
