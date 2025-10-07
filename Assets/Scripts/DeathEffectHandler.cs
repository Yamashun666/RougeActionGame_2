using System.Collections;
using UnityEngine;

/// <summary>
/// HPが0になったときに死亡演出を再生し、オブジェクトを破壊する
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class DeathEffectHandler : MonoBehaviour
{
    [Header("死亡時に再生するVFX")]
    public string deathVFXName;

    [Header("死亡時に再生するSFX")]
    public string deathSFXName;

    [Header("削除までの遅延(秒)")]
    public float destroyDelay = 0.5f;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isDead = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// 外部から死亡トリガーを呼ぶ
    /// </summary>
    public void TriggerDeath()
    {
        if (isDead) return; // 二重死防止
        isDead = true;
        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {
        // 1️⃣ スプライトをフェードアウト
        float fadeDuration = 0.3f;
        float elapsed = 0f;
        Color startColor = spriteRenderer.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // 2️⃣ VFX再生
        if (!string.IsNullOrEmpty(deathVFXName))
        {
            GameObject vfxPrefab = Resources.Load<GameObject>(deathVFXName);
            if (vfxPrefab != null)
            {
                Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            }
        }

        // 3️⃣ SFX再生
        if (!string.IsNullOrEmpty(deathSFXName))
        {
            AudioClip clip = Resources.Load<AudioClip>(deathSFXName);
            if (clip != null) audioSource.PlayOneShot(clip);
        }

        // 4️⃣ 破壊まで少し待機
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
