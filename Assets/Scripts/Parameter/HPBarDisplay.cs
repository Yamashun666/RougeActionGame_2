using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HPをImageのFillAmountで表示する共通クラス
/// ParameterBase と紐づけて自動更新
/// </summary>
[RequireComponent(typeof(Image))]
public class HPBarDisplay : MonoBehaviour
{
    [Header("参照するキャラのパラメータ")]
    public ParameterBase targetParameter;

    [Header("遅延ゲージ（ダメージ残像演出）を付けたい場合")]
    public Image delayedBar; // 任意（無くても動く）
    public float smoothSpeed = 2.0f;

    private Image hpBar;
    private float currentFill = 1f;

    void Awake()
    {
        hpBar = GetComponent<Image>();
    }

    void Update()
    {
        if (targetParameter == null) return;

        float targetFill = Mathf.Clamp01(
            (float)targetParameter.CurrentHP / targetParameter.MaxHP
        );

        // 即時更新バー
        hpBar.fillAmount = targetFill;

        // 遅延バー演出（ダメージを受けたときにゆっくり減る）
        if (delayedBar != null)
        {
            if (delayedBar.fillAmount > targetFill)
                delayedBar.fillAmount = Mathf.Lerp(delayedBar.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
            else
                delayedBar.fillAmount = targetFill;
        }

        currentFill = targetFill;
    }
}
