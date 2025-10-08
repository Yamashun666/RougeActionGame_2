using UnityEngine;

/// <summary>
/// キャラクター生成時にHPバー（3層式）をCanvasへ自動生成して紐付ける
/// 敵・味方共通で使用可能
/// </summary>
public class HPBarBinder : MonoBehaviour
{
    [Header("参照設定")]
    public ParameterBase parameter;
    public GameObject hpBarPrefab;  // HPBarUI_Tripleを含むPrefab
    private GameObject instance;

    private void Start()
    {
        if (parameter == null)
            parameter = GetComponent<ParameterBase>();

        // Canvas取得（ワールド空間・スクリーン空間どちらでもOK）
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[HPBarBinder] Canvasがシーンに存在しません。");
            return;
        }

        // HPバー生成
        instance = Instantiate(hpBarPrefab, canvas.transform);
        instance.name = $"{parameter.Name}_HPBar";

        // 各スクリプトへの紐付け
        HPBarUI_Triple bar = instance.GetComponent<HPBarUI_Triple>();
        HPBarFollow follow = instance.GetComponent<HPBarFollow>();

        if (bar != null)
            bar.target = parameter;

        if (follow != null)
            follow.target = parameter.ModelRoot;

        Debug.Log($"[HPBarBinder] {parameter.Name} にHPバーを生成しました。");
    }

    private void OnDestroy()
    {
        if (instance != null)
            Destroy(instance);
    }
}
