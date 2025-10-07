using System.Collections;
using UnityEngine;

/// <summary>
/// 被弾時にスプライトを白点滅させる演出
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlashOnDamage : MonoBehaviour
{
    [Header("点滅の長さ(秒)")]
    public float flashDuration = 0.15f;

    [Header("点滅の回数")]
    public int flashCount = 2;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// 被弾時に呼び出す
    /// </summary>
    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration / (flashCount * 2f));
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration / (flashCount * 2f));
        }
    }
}
