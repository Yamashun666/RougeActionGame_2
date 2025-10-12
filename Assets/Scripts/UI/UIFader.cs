using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIFader : MonoBehaviour
{
    [Header("フェード対象（空なら子のImageを自動検出）")]
    [SerializeField] private Image[] targetImages;

    [Header("フェード時間（秒）")]
    [SerializeField] private float fadeTime = 0.5f;

    [Header("開始時にフェードアウト状態にするか")]
    [SerializeField] private bool startFadedOut = true;

    private void Awake()
    {
        // 空なら子オブジェクトから自動取得
        if (targetImages == null || targetImages.Length == 0)
            targetImages = GetComponentsInChildren<Image>();

        // フェードアウト開始指定があれば、透明にしておく
        if (startFadedOut)
        {
            foreach (var img in targetImages)
            {
                if (img != null)
                {
                    Color c = img.color;
                    c.a = 0f; // 完全に透明
                    img.color = c;
                }
            }
        }
    }

    public void UIFadeOut()
    {
        foreach (var img in targetImages)
        {
            if (img != null)
                img.DOFade(0f, fadeTime);
        }
    }

    public void UIFadeIn()
    {
        foreach (var img in targetImages)
        {
            if (img != null)
                img.DOFade(1f, fadeTime);
        }
    }
}
