using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDropField : MonoBehaviour, IDropHandler
{
    public Transform dropSpawnRoot;
    public GameObject skillOrbPrefab;

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<SkillOrbUI>();
        if (dragged == null) return;

        var originSlot = dragged.GetOriginSlot();
        if (originSlot == null)
        {
            Debug.LogWarning("[SkillDropField] originSlot が null。");
            return;
        }

        // スキルオーブをフィールドに生成
        var drop = dragged.GetDroppedItem();
        if (drop == null)
        {
            Debug.LogError("[SkillDropField] DroppedItem が null。");
            return;
        }

        GameObject spawned = Instantiate(skillOrbPrefab, dropSpawnRoot.position, Quaternion.identity);
        spawned.GetComponent<DroppedItem>().AssignSkill(drop.GetAssignedSkill());

        // 元スロットから削除
        originSlot.ClearSkill();

        Debug.Log($"[SkillDropField] {drop.GetAssignedSkill().SkillName} をフィールドにドロップしました。");
    }
}
