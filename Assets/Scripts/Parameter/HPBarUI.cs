using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [Header("参照")]
    public ParameterBase target;  // HP監視対象
    public Image hpFill;          // HPゲージ本体

    private void Update()
    {
        if (target == null || hpFill == null) return;

        float totalHP = target.MaxHP + target.LimitOverHP;
        float currentHP = target.CurrentHP + target.LimitOverHP;
        float ratio = Mathf.Clamp01(currentHP / Mathf.Max(totalHP, 1f));
        hpFill.fillAmount = ratio;
    }
    private void OnEnable()
    {
        if (target != null)
            target.OnDeath += Hide;
    }

    private void OnDisable()
    {
        if (target != null)
            target.OnDeath -= Hide;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
