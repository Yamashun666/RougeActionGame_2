using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HPバー（赤バー＋白バーの遅延アニメ付き）
/// </summary>
public class HPBarUI_WithDelay : MonoBehaviour
{
    [Header("参照")]
    public ParameterBase target;
    public Image hpFill;         // 即時HP（赤）
    public Image delayedFill;    // 遅延HP（白）

    [Header("設定")]
    public float delaySpeed = 1.5f; // 白バーが追いつく速さ
    public bool hideWhenDead = true;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("[HPBarUI] target が未設定です。");
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
        float currentHP = target.CurrentHP;

        // 即時バー（赤）を現在値で更新
        float currentRatio = Mathf.Clamp01(currentHP / Mathf.Max(totalHP, 1f));
        hpFill.fillAmount = currentRatio;

        // 遅延バー（白）は滑らかに追従
        if (delayedFill.fillAmount > currentRatio)
        {
            delayedFill.fillAmount = Mathf.Lerp(delayedFill.fillAmount, currentRatio, Time.deltaTime * delaySpeed);
        }
        else
        {
            delayedFill.fillAmount = currentRatio; // 回復時は即時追いつく
        }
    }

    private void HandleDeath()
    {
        if (hideWhenDead)
            gameObject.SetActive(false);
    }
}
