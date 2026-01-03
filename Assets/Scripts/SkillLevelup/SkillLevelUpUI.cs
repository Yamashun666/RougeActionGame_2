using UnityEngine;

public class SkillLevelUpUI : MonoBehaviour
{
    public SkillSlotUI[] slots;
    public SkillLevelUpService service;

    public void Open()
    {
        gameObject.SetActive(true);
        // UIにスロットのスキルを表示
    }

    public void OnClickSkillSlot(int index)
    {
        service.TryLevelUp(slots[index]);
    }
}

