using UnityEngine;
using System.Collections.Generic;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance { get; private set; }

    [Header("UI生成設定")]
    [Tooltip("SkillOrbUIプレハブのResources内パス。例: Resources/UI/SkillOrbUI.prefab")]
    [SerializeField] private string prefabPath = "UI/SkillOrbUI";

    [Tooltip("SkillOrbUIを配置する親（Canvas配下推奨）")]
    [SerializeField] private Transform skillListParent;

    private GameObject skillOrbPrefab;
    private readonly List<SkillOrbUI> activeSkillUIList = new();

    private void Awake()
    {
        // シングルトン処理
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadPrefab();

        // Canvas自動検出（未指定なら）
        if (skillListParent == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                skillListParent = canvas.transform;
                Debug.Log("[SkillUIManager] Canvasが自動検出されました。");
            }
            else
            {
                Debug.LogError("[SkillUIManager] Canvasが見つかりません。UI生成に失敗します。");
            }
        }
    }

    /// <summary>
    /// ResourcesからPrefabをロード
    /// </summary>
    private void LoadPrefab()
    {
        skillOrbPrefab = Resources.Load<GameObject>(prefabPath);
        if (skillOrbPrefab == null)
        {
            Debug.LogError($"[SkillUIManager] {prefabPath}.prefab が Resources フォルダ内に存在しません！");
        }
        else
        {
            Debug.Log($"[SkillUIManager] SkillOrbUI プレハブをロード成功：{prefabPath}");
        }
    }

    /// <summary>
    /// 新しいスキルUIアイコンを生成
    /// </summary>
    public void CreateSkillOrbUI(SkillData skillData)
    {
        if (skillData == null)
        {
            Debug.LogError("[SkillUIManager] SkillDataがnullです。");
            return;
        }

        if (skillOrbPrefab == null)
        {
            LoadPrefab();
            if (skillOrbPrefab == null) return;
        }

        Transform parent = skillListParent != null ? skillListParent : transform;
        GameObject uiObj = Instantiate(skillOrbPrefab, parent);

        uiObj.transform.localScale = Vector3.one; // スケール崩壊対策
        uiObj.transform.localPosition = Vector3.zero;

        var orbUI = uiObj.GetComponent<SkillOrbUI>();
        if (orbUI != null)
        {
            orbUI.SetSkill(skillData);
            activeSkillUIList.Add(orbUI);
            Debug.Log($"[SkillUIManager] スキルUI生成完了：{skillData.SkillName}");
        }
        else
        {
            Debug.LogError("[SkillUIManager] SkillOrbUIがプレハブに存在しません。");
        }
    }

    /// <summary>
    /// すべてのスキルUIをクリア
    /// </summary>
    public void ClearAllSkillUI()
    {
        foreach (var ui in activeSkillUIList)
        {
            if (ui != null) Destroy(ui.gameObject);
        }
        activeSkillUIList.Clear();
        Debug.Log("[SkillUIManager] スキルUIを全削除しました。");
    }
}
