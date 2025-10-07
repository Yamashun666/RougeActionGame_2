using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DeathEffectHandler : MonoBehaviour
{
    public string deathVFXName;
    public string deathSFXName;
    public float destroyDelay = 0.5f;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private ParameterBase parameterBase;
    private bool isDead = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        parameterBase = GetComponent<ParameterBase>();
    }

    void OnEnable()
    {
        if (parameterBase != null)
            parameterBase.OnDeath += TriggerDeath;
    }

    void OnDisable()
    {
        if (parameterBase != null)
            parameterBase.OnDeath -= TriggerDeath;
    }

    public void TriggerDeath()
    {
        if (isDead) return;
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
                Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        }

        // 3️⃣ SFX再生
        if (!string.IsNullOrEmpty(deathSFXName))
        {
            AudioClip clip = Resources.Load<AudioClip>(deathSFXName);
            if (clip != null)
                audioSource.PlayOneShot(clip);
        }

        // 4️⃣ 削除まで遅延
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
