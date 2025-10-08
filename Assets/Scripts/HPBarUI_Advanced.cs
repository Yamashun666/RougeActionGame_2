using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HPバー（通常HP + LimitOverHP の2層構造）
/// </summary>
public class HPBarUI_Advanced : MonoBehaviour
{
    [Header("参照")]
    public ParameterBase target;      // HP参照元
    public Image hpFill;              // 通常HP（赤）
    public Image limitOverFill;       // LimitOverHP（青）

    [Header("設定")]
    public bool hideWhenDead = true;  // 死亡時非表示
    private bool initialized = false;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning($"[HPBarUI] target が未設定です。");
            return;
        }

        target.OnDeath += HandleDeath;
        initialized = true;
    }

    private void OnDestroy()
    {
        if (target != null)
            target.OnDeath -= HandleDeath;
    }

    private void Update()
    {
        if (!initialized || target == null) return;

        float totalMax = target.MaxHP;
        float limitOver = target.LimitOverHP;
        float current = target.CurrentHP;

        float hpRatio = Mathf.Clamp01((float)current / Mathf.Max(totalMax, 1f));
        float limitOverRatio = Mathf.Clamp01((float)limitOver / Mathf.Max(totalMax, 1f));

        // 通常HPバー更新
        if (hpFill != null)
            hpFill.fillAmount = hpRatio;

        // 上限超過バー更新
        if (limitOverFill != null)
            limitOverFill.fillAmount = Mathf.Min(hpRatio + limitOverRatio, 1f);
    }

    private void HandleDeath()
    {
        if (hideWhenDead)
            gameObject.SetActive(false);
    }
}
