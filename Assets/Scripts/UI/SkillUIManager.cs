using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance { get; private set; }
    public SkillOrbDragController skillOrbDragController;

    [Header("UIスロット（Q=0, W=1, E=2, R=3 の想定）")]
    public SkillSlotUI[] slots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning("[SkillUIManager] slots が設定されていません。Inspectorで4つの SkillSlotUI を割り当ててください。");
        }
    }

    /// <summary>
    /// 最初に空いているスロットへ登録
    /// </summary>
    public void RegisterSkillToNextSlot(SkillData skill, DroppedItem dropItem)
    {
        if (skill == null)
        {
            Debug.LogError("[SkillUIManager] skill が null です。");
            return;
        }

        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("[SkillUIManager] slots が未設定です。");
            return;
        }

        foreach (var slot in slots)
        {
            if (slot == null) continue;

            if (slot.assignedSkill == null)
            {
                slot.SetSkill(skill, dropItem, skillOrbDragController.cachedIcon);
                Debug.Log($"[SkillUIManager] {skill.SkillName} をスロット {slot.slotIndex} に登録しました。");
                return;
            }
        }

        Debug.LogWarning("[SkillUIManager] 空きスロットがありません。");
    }

    /// <summary>
    /// スロット番号を指定して登録（必要なら使用）
    /// </summary>
    public void RegisterSkillToSlot(int index, SkillData skill, DroppedItem dropItem)
    {
        if (slots == null || index < 0 || index >= slots.Length)
        {
            Debug.LogError($"[SkillUIManager] 不正なスロット index={index}");
            return;
        }
        slots[index].SetSkill(skill, dropItem, skillOrbDragController.cachedIcon);
    }
}
