// デバッグテスト用
using UnityEngine;
public class TestSkillUISetup : MonoBehaviour
{
    public SkillSlotUI slot;
    SkillOrbDragController skillOrbDragController;

    void Start()
    {
        SkillData dummy = ScriptableObject.CreateInstance<SkillData>();
        DroppedItem dummy2 = GetComponent<DroppedItem>();
        dummy.SkillName = "Fireball";
        dummy.SkillIcon = "fireball_icon"; // Resources/SkillIcons/fireball_icon.png

        slot.SetSkill(dummy,dummy2,skillOrbDragController.cachedIcon );
    }
}
