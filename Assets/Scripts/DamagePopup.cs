using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float floatSpeed = 1.5f;
    public float fadeDuration = 0.8f;
    public float lifeTime = 1f;

    public Color damageColor = Color.red;
    public Color healColor = Color.green;
    public Color criticalColor = Color.yellow;

    private TextMeshProUGUI textMesh;
    private CanvasGroup canvasGroup;
    private Vector3 moveDir;
    private float timer;

    public void Initialize(int value, bool isHeal = false, bool isCritical = false)
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (textMesh == null)
        {
            Debug.LogError("[DamagePopup] TextMeshProUGUIが見つかりません");
            return;
        }

        // 色・サイズ・内容
        textMesh.text = (isHeal ? "+" : "-") + value.ToString();
        textMesh.color = isCritical ? criticalColor : (isHeal ? healColor : damageColor);
        textMesh.fontSize = isCritical ? 60 : 45;
        canvasGroup.alpha = 1f;

        // ランダムに少しだけ傾ける
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-5f, 5f));
        moveDir = new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0f).normalized;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        transform.position += moveDir * floatSpeed * Time.deltaTime;

        // フェードアウト
        if (timer > lifeTime - fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, (timer - (lifeTime - fadeDuration)) / fadeDuration);
        }

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}
