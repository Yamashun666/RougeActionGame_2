using UnityEngine;
using TMPro;

/// <summary>
/// HPをTextMeshProで表示する共通クラス
/// ParameterBase と紐づけて自動更新
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class HPTextDisplay : MonoBehaviour
{
    [Header("参照するキャラのパラメータ")]
    public ParameterBase targetParameter;

    private TextMeshProUGUI hpText;

    void Awake()
    {
        hpText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (targetParameter == null) return;

        // 現在HP / 最大HP の形式で表示
        hpText.text = $"{targetParameter.CurrentHP} / {targetParameter.MaxHP}";
    }
}
