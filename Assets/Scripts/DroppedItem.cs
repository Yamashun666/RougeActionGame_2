using System.Collections;
using UnityEngine;

/// <summary>
/// 敵からドロップしたスキルオーブやアイテムを表すクラス。
/// </summary>
public class DroppedItem : MonoBehaviour
{
    [Header("ビジュアル関連")]
    public SpriteRenderer iconRenderer;
    public Color commonColor = Color.white;
    public Color unCommonColor = Color.gray;
    public Color rareColor = Color.cyan;
    public Color epicColor = Color.magenta;
    public Color legendaryColor = Color.yellow;
    public SkillData skillData;
    public Sprite defaultIcon;
    private SpriteRenderer spriteRenderer;

    public SkillData GetAssignedSkill() => skillData;
    public SpriteRenderer IconRenderer;   // Orbの見た目
    public PlayerInventory inventory;

    [Header("内部データ")]
    public string skillLevelCode; // SkillDatabase内のLevelCode
    [Header("見た目/データ")]
    private float spawnTime;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Rigidbodyを強制的にKinematic固定
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }

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
    public void Initialize(SkillData data)
    {
        skillData = data;
        Debug.Log($"[DroppedItem] {skillData.SkillName} を保持したOrb生成");
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null)
            inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning($"[DroppedItem] {other.name} に PlayerInventory が見つからない");
            return;
        }

        inventory.RegisterNearbyItem(this);  // ← ★ここで渡してOK！
        Debug.Log($"[DroppedItem] RegisterNearbyItem 呼び出し: {this.name}");
        if (inventory == null)
        {
            Debug.LogWarning("[DroppedItem] PlayerInventory not found");
        }
        else
        {
            Debug.Log($"[DroppedItem] RegisterNearbyItem 呼び出し: {this.name}, inventory={inventory.gameObject.name}");
            inventory.RegisterNearbyItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        StartCoroutine(DelayedUnregister(other));
    }

    private IEnumerator DelayedUnregister(Collider2D other)
    {
        yield return new WaitForSeconds(0.2f); // ← 0.2秒後に確認
        var inventory = other.GetComponentInParent<PlayerInventory>();
        if (inventory == null) yield break;

        if (inventory != null && Vector2.Distance(other.transform.position, transform.position) > 1f)
        {
            inventory.UnregisterNearbyItem(this);
            Debug.Log("[DroppedItem] PlayerInventory から解除しました（遅延確認後）");
        }
    }



    void OnEnable()
    {
        spawnTime = Time.time;
    }
    private void OnDestroy()
    {
        Debug.Log($"[DroppedItem] {name} が破棄されました。");
    }

}
