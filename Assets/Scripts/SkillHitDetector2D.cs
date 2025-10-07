using UnityEngine;
using Game.SkillSystem;

public class SkillHitDetector2D
{
    private Vector2 lastOrigin;
    private Vector2 lastDirection;
    private HitShape lastShape;
    private float lastRange;
    private Vector2 lastSize;

    // AudioSource（再生用）を外部から渡す or 自身で生成
    private AudioSource audioSource;

    public SkillHitDetector2D()
    {
        GameObject audioObj = new GameObject("HitSFXPlayer");
        audioSource = audioObj.AddComponent<AudioSource>();
    }

    public void PerformHitDetection(SkillInstance instance)
    {
        Vector2 origin = instance.Caster.Position;
        Vector2 baseDirection = Vector2.right;
        float facing = Mathf.Sign(instance.Caster.transform.localScale.x);
        Vector2 direction = baseDirection * facing;

        float range = 3f;
        Vector2 size = new Vector2(1f, 1f);
        float radius = 0.5f;

        lastOrigin = origin;
        lastDirection = direction;
        lastShape = instance.Data.HitShapeType;
        lastRange = range;
        lastSize = size;

        HitShape shape = instance.Data.HitShapeType;
        Collider2D[] hits = System.Array.Empty<Collider2D>();

        switch (shape)
        {
            case HitShape.Box:
                hits = Physics2D.OverlapBoxAll(origin + direction * range / 2f, size, 0f);
                break;

            case HitShape.Capsule:
                hits = Physics2D.OverlapCapsuleAll(origin, size, CapsuleDirection2D.Vertical, 0f);
                break;

            case HitShape.Ray:
                RaycastHit2D[] rayHits = Physics2D.RaycastAll(origin, direction, range);
                hits = new Collider2D[rayHits.Length];
                for (int i = 0; i < rayHits.Length; i++)
                    hits[i] = rayHits[i].collider;
                break;
        }

        foreach (var col in hits)
        {
            ParameterBaseHolder holder = col.GetComponent<ParameterBaseHolder>();
            if (holder?.Parameter != null)
            {
                holder.Parameter.TakeDamage(instance.Data.EffectAmount001);
                Debug.Log($"[2D HIT] {col.name} に {instance.Data.EffectAmount001} ダメージ");

                // 命中時VFX
                PlayHitVFX(instance, col.transform.position);
                // 命中時SFX
                PlayHitSFX(instance);
            }
        }
    }

    private void PlayHitVFX(SkillInstance instance, Vector3 hitPosition)
    {
        string vfxName = instance.Data.UseSkillVFX001;
        if (string.IsNullOrEmpty(vfxName)) return;

        GameObject vfxPrefab = Resources.Load<GameObject>(vfxName);
        if (vfxPrefab != null)
        {
            GameObject effect = Object.Instantiate(vfxPrefab, hitPosition, Quaternion.identity);
            Object.Destroy(effect, 2f);
        }
    }

    private void PlayHitSFX(SkillInstance instance)
    {
        string sfxName = instance.Data.UseSkillSFX001;
        if (string.IsNullOrEmpty(sfxName)) return;

        AudioClip clip = Resources.Load<AudioClip>(sfxName);
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[SkillHitDetector2D] SFX '{sfxName}' が見つかりません");
        }
    }

#if UNITY_EDITOR
    public void DrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 center = lastOrigin + lastDirection * lastRange / 2f;

        if (lastShape == HitShape.Box)
            Gizmos.DrawWireCube(center, lastSize);
        else if (lastShape == HitShape.Capsule)
        {
            Gizmos.DrawWireSphere(lastOrigin, lastSize.x / 2);
            Gizmos.DrawWireSphere(lastOrigin + lastDirection * lastRange, lastSize.x / 2);
        }
        else if (lastShape == HitShape.Ray)
            Gizmos.DrawLine(lastOrigin, lastOrigin + lastDirection * lastRange);
    }
#endif
}
