using UnityEngine;

public class SkillLevelUpEvent : MonoBehaviour
{
    public SkillLevelUpUI levelUpUI;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            levelUpUI.Open();
        }
    }
}

