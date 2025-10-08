using System.Collections;
using UnityEngine;

/// <summary>
/// キャラクター死亡時の演出・破棄処理を一元管理するハンドラー。
/// ParameterBase.OnDeath イベントに自動で登録される。
/// </summary>
public class DeathEffectHandler : MonoBehaviour
{
    [Header("エフェクト関連")]
    public GameObject deathVFXPrefab;    // 死亡時のエフェクトPrefab
    public AudioClip deathSFX;           // 死亡時の効果音
    public float destroyDelay = 1.5f;    // 破棄までの遅延時間

    private AudioSource audioSource;
    private ParameterBase parameter;

    private void Awake()
    {
        parameter = GetComponent<ParameterBase>();
        audioSource = GetComponent<AudioSource>();

        if (parameter != null)
            parameter.OnDeath += TriggerDeath; // ← イベント登録
    }

    private void OnDestroy()
    {
        if (parameter != null)
            parameter.OnDeath -= TriggerDeath;
    }

    /// <summary>
    /// 死亡演出の呼び出し（ParameterBase.OnDeath から呼ばれる）
    /// </summary>
    public void TriggerDeath()
    {
        StartCoroutine(HandleDeathSequence());
    }

    /// <summary>
    /// 実際の死亡シーケンス処理
    /// </summary>
    private IEnumerator HandleDeathSequence()
    {
        // VFX再生
        if (deathVFXPrefab != null)
        {
            GameObject vfx = Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        // SFX再生
        if (deathSFX != null)
        {
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.PlayOneShot(deathSFX);
        }

        // HPバーなどUI破棄（自動で消したい場合）
        HPBarUI_Triple bar = FindObjectOfType<HPBarUI_Triple>();
        if (bar != null && bar.target == parameter)
            Destroy(bar.gameObject);

        // 一定時間待ってからキャラ削除
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
