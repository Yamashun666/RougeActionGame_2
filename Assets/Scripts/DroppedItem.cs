using UnityEngine;

/// <summary>
/// 敵からドロップしたスキルオーブやアイテムを表すクラス。
/// </summary>
public class DroppedItem : MonoBehaviour
{
    [Header("ビジュアル関連")]
    public SpriteRenderer iconRenderer;
    public Sprite defaultIcon;
    public Color commonColor = Color.white;
    public Color rareColor = Color.cyan;
    public Color epicColor = Color.magenta;
    public Color legendaryColor = Color.yellow;
    public SkillData skillData;

    [Header("内部データ")]
    public string skillLevelCode; // SkillDatabase内のLevelCode
    private SkillData assignedSkill;

    private void Start()
    {
        // デバッグ用
        if (assignedSkill != null)
            Debug.Log($"[DroppedItem] {assignedSkill.SkillName} がセットされています。");
    }

    /// <summary>
    /// スキル情報を割り当てる
    /// </summary>
    public void AssignSkill(string levelCode)
    {
        assignedSkill = SkillDatabase.Instance.GetSkill(levelCode);
        if (assignedSkill != null)
            Debug.Log($"💎 DroppedItem にスキル [{assignedSkill.SkillName}] を割り当てました！");
    }
    public SkillData GetAssignedSkill()
    {
        return assignedSkill;
    }

    /// <summary>
    /// レアリティなどに応じて見た目を設定
    /// </summary>
    private void ApplyVisuals()
    {
        if (iconRenderer == null)
            iconRenderer = GetComponentInChildren<SpriteRenderer>();

        if (assignedSkill == null)
        {
            iconRenderer.sprite = defaultIcon;
            iconRenderer.color = commonColor;
            return;
        }

        // アイコン画像をロード
        Sprite skillSprite = Resources.Load<Sprite>($"Icons/{assignedSkill.SkillIcon}");
        if (skillSprite != null)
            iconRenderer.sprite = skillSprite;

        // レアリティ別カラー反映
        switch (assignedSkill.Rarity)
        {
            case 1: iconRenderer.color = commonColor; break;
            case 2: iconRenderer.color = rareColor; break;
            case 3: iconRenderer.color = epicColor; break;
            case 4: iconRenderer.color = legendaryColor; break;
            default: iconRenderer.color = commonColor; break;
        }
    }

}
