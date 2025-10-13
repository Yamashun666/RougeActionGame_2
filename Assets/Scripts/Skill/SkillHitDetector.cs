using UnityEngine;
using Game.SkillSystem;

/// <summary>
/// 2Dアクション用スキルヒット判定クラス。
/// Gizmosを利用してSceneビューで攻撃範囲を可視化。
/// </summary>
public class SkillHitDetector
{
    private int enemyLayerMask = -1;

    // デバッグ用キャッシュ（最後の判定位置を描画）
    private Vector2 lastOrigin;
    private Vector2 lastDirection;
    private float lastRange;
    private Vector2 lastSize;

    public void InitializeLayerMask()
    {
        if (enemyLayerMask == -1)
            enemyLayerMask = LayerMask.GetMask("Enemy");
    }

    public void PerformHitDetection(SkillInstance instance, Transform originTransform)
    {
        if (instance == null || originTransform == null)
        {
            Debug.LogWarning("[SkillHitDetector] Invalid call: instance or originTransform is null");
            return;
        }

        Vector2 origin = originTransform.position;
        Vector2 direction = originTransform.localScale.x > 0 ? Vector2.right : Vector2.left;
        float range = 3f;
        Vector2 size = new Vector2(1f, 1f);

        // === デバッグ用キャッシュ保存 ===
        lastOrigin = origin;
        lastDirection = direction;
        lastRange = range;
        lastSize = size;

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(origin + direction * range / 2f, size, 0f, enemyLayerMask);

        foreach (var col in hitColliders)
        {
            if (col == null || col.CompareTag("Ground")) continue;

            var targetParam = col.GetComponent<ParameterBaseHolder>()?.Parameter;
            if (targetParam != null)
            {
                int before = targetParam.CurrentHP;
                targetParam.TakeDamage(instance.Data.EffectAmount001);
                Debug.Log($"[Hit2D] {col.name} に {instance.Data.EffectAmount001} ダメージ！ {before} → {targetParam.CurrentHP}");
            }
        }
    }

    /// <summary>
    /// Sceneビューで攻撃範囲を可視化
    /// </summary>
    public void DrawGizmos()
    {
        if (lastRange <= 0) return;

        Gizmos.color = Color.red;
        Vector3 center = lastOrigin + lastDirection * lastRange / 2f;
        Gizmos.DrawWireCube(center, lastSize);
        Gizmos.DrawLine(lastOrigin, lastOrigin + lastDirection * lastRange);
    }
}
