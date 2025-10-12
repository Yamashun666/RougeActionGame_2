using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 三層HPバー（LimitOver + 遅延白バー + 現在HP）
/// 敵・プレイヤー共通
/// </summary>
public class HPBarUI_Triple : MonoBehaviour
{
    [Header("参照")]
    public ParameterBase target;
    public Image hpFill;          // 現在HP（赤）
    public Image delayedFill;     // 遅延HP（白）
    public Image limitOverFill;   // LimitOverHP（青）

    [Header("設定")]
    public float delaySpeed = 1.5f;
    public bool hideWhenDead = true;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("[HPBarUI_Triple] target 未設定");
            return;
        }

        target.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        if (target != null)
            target.OnDeath -= HandleDeath;
    }

    private void Update()
    {
        if (target == null) return;

        float totalHP = target.MaxHP;
        float limitOver = target.LimitOverHP;
        float currentHP = target.CurrentHP;

        float hpRatio = Mathf.Clamp01(currentHP / Mathf.Max(totalHP, 1f));
        float delayedRatio = Mathf.Clamp01(delayedFill.fillAmount);
        float limitOverRatio = Mathf.Clamp01(limitOver / Mathf.Max(totalHP, 1f));

        // LimitOver（青バー）
        if (limitOverFill != null)
            limitOverFill.fillAmount = Mathf.Min(hpRatio + limitOverRatio, 1f);

        // 即時HP（赤バー）
        if (hpFill != null)
            hpFill.fillAmount = hpRatio;

        // 遅延白バー
        if (delayedFill != null)
        {
            if (delayedFill.fillAmount > hpRatio)
                delayedFill.fillAmount = Mathf.Lerp(delayedFill.fillAmount, hpRatio, Time.deltaTime * delaySpeed);
            else
                delayedFill.fillAmount = hpRatio; // 回復は即追いつく
        }
    }

    private void HandleDeath()
    {
        if (hideWhenDead)
            gameObject.SetActive(false);
    }
}
