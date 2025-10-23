// デバッグテスト用
using UnityEngine;
public class TestSkillUISetup : MonoBehaviour
{
    public SkillSlotUI slot;

    void Start()
    {
        SkillData dummy = ScriptableObject.CreateInstance<SkillData>();
        dummy.SkillName = "Fireball";
        dummy.SkillIcon = "fireball_icon"; // Resources/SkillIcons/fireball_icon.png

        slot.SetSkill(dummy);
    }
}
