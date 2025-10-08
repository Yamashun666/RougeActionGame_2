using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーの固定UI HPバー（画面左上など）
/// </summary>
public class HUD_PlayerHP : MonoBehaviour
{
    [Header("参照")]
    public ParameterBase playerParam;
    public Image hpFill;
    public Image limitOverFill;

    private void Update()
    {
        if (playerParam == null) return;

        float totalMax = playerParam.MaxHP;
        float limitOver = playerParam.LimitOverHP;
        float current = playerParam.CurrentHP;

        float hpRatio = Mathf.Clamp01(current / Mathf.Max(totalMax, 1f));
        float limitOverRatio = Mathf.Clamp01(limitOver / Mathf.Max(totalMax, 1f));

        hpFill.fillAmount = hpRatio;
        limitOverFill.fillAmount = Mathf.Min(hpRatio + limitOverRatio, 1f);
    }
}
