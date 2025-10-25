using UnityEngine;

/// <summary>
/// 敵からドロップしたスキルオーブやアイテムを表すクラス。
/// </summary>
public class DroppedItem : MonoBehaviour
{
    [Header("ビジュアル関連")]
    public SpriteRenderer iconRenderer;
    public Color commonColor = Color.white;
    public Color rareColor = Color.cyan;
    public Color epicColor = Color.magenta;
    public Color legendaryColor = Color.yellow;
    public SkillData skillData;
    public Sprite defaultIcon;

    public SkillData GetAssignedSkill() => skillData;

    [Header("内部データ")]
    public string skillLevelCode; // SkillDatabase内のLevelCode



    private void Awake()
    {
        // テスト用: 未設定ならResourcesから仮読み
        if (skillData == null)
        {
            skillData = Resources.Load<SkillData>("SkillDatabase/DoubleJump");
            if (skillData == null)
                Debug.LogWarning("[DroppedItem] SkillData未設定＆Resourcesに該当スキルなし");
        }
    }
    private void Start()
    {
        // デバッグ用
        if (skillData != null)
            Debug.Log($"[DroppedItem] {skillData.SkillName} がセットされています。");
    }

    /// <summary>
    /// スキル情報を割り当てる
    /// </summary>
    public void AssignSkill(SkillData skill)
    {
        Debug.Log("[DroppedItem.AssignSkill]Called AssignSkill");
        skillData = skill;
        defaultIcon = skill != null ? Resources.Load<Sprite>(skill.SkillIcon) : null;
        if (defaultIcon == null)
        {
            Debug.LogWarning($"[DroppedItem] SkillIconが見つかりません: {skill.SkillIcon}");
        }
    }

    /// <summary>
    /// レアリティなどに応じて見た目を設定
    /// </summary>
    private void ApplyVisuals()
    {
        if (iconRenderer == null)
            iconRenderer = GetComponentInChildren<SpriteRenderer>();

        if (skillData == null)
        {
            iconRenderer.sprite = defaultIcon;
            iconRenderer.color = commonColor;
            return;
        }

        // アイコン画像をロード
        Sprite skillSprite = Resources.Load<Sprite>($"Icons/{skillData.SkillIcon}");
        if (skillSprite != null)
            iconRenderer.sprite = skillSprite;

        // レアリティ別カラー反映
        switch (skillData.Rarity)
        {
            case 1: iconRenderer.color = commonColor; break;
            case 2: iconRenderer.color = rareColor; break;
            case 3: iconRenderer.color = epicColor; break;
            case 4: iconRenderer.color = legendaryColor; break;
            default: iconRenderer.color = commonColor; break;
        }
    }

}
