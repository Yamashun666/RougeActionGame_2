using UnityEngine;
using Game.SkillSystem;
using System;

/// <summary>
/// 【クラス名】SkillHitDetector
/// 役割：スキルの当たり判定を行う（2D用）
/// ※Tilemapレイヤーを無視し、実際のキャラクター座標に合わせて補正する
/// </summary>
public class SkillHitDetector
{
    private int enemyLayerMask = -1;

    // デバッグキャッシュ（Gizmos可視化用）
    private Vector2 lastOrigin;
    private Vector2 lastDirection;
    private float lastRange;
    private Vector2 lastSize;
    private bool hasCache = false;

    /// <summary>
    /// レイヤーマスク初期化
    /// </summary>
    public void InitializeLayerMask()
    {
        if (enemyLayerMask == -1)
            enemyLayerMask = LayerMask.GetMask("Enemy");
    }

    /// <summary>
    /// スキルのヒット判定を実行
    /// </summary>
    /// <param name="instance">スキル情報</param>
    /// <param name="originTransform">攻撃発生元（例：プレイヤー）</param>
    public void PerformHitDetection(SkillInstance instance, Transform originTransform)
    {
        if (instance == null || originTransform == null)
        {
            Debug.LogWarning("[SkillHitDetector] Invalid call: instance or originTransform is null");
            return;
        }

        // ======================
        // 座標取得・補正
        // ======================
        Vector2 origin = originTransform.position;

        // ParameterBase.ModelRoot優先
        var param = originTransform.GetComponent<ParameterBase>();
        if (param != null && param.ModelRoot != null)
            origin = param.ModelRoot.position;

        // Tilemap環境ではワールドスケールが1000以上になる場合があるので補正
        if (origin.magnitude > 100f)
        {
            origin /= 1000000f;
            Debug.LogWarning("[HitDetector] 座標補正適用：Tilemapスケールが大きい可能性あり。");
        }

        // 攻撃方向はlocalScale.xで決定
        Vector2 direction = originTransform.localScale.x >= 0 ? Vector2.right : Vector2.left;
        float range = 3f;
        Vector2 size = new Vector2(3f, 2f);

        Debug.Log($"[DEBUG] origin={origin}, direction={direction}, range={range}");

        // 赤線でデバッグ可視化
        Debug.DrawLine(origin, origin + (Vector2)(direction * range), Color.red, 1.5f);


        // ======================
        // 判定実行
        // ======================
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            origin + (Vector2)(direction * range / 2f),
            size,
            0f,
            enemyLayerMask
        );

        Debug.Log($"[DEBUG] OverlapBoxAll result count={hitColliders.Length}");

        // デバッグキャッシュ（Gizmos用）
        lastOrigin = origin;
        lastDirection = direction;
        lastRange = range;
        lastSize = size;
        hasCache = true;

        // ======================
        // ヒット処理
        // ======================
        foreach (var col in hitColliders)
        {
            if (col == null) continue;
            if (col.CompareTag("Ground") || col.gameObject.layer == LayerMask.NameToLayer("Ground")) continue;

            Debug.Log($"[HitDetector] 衝突: {col.name}, layer={LayerMask.LayerToName(col.gameObject.layer)}");

            var targetParam = col.GetComponent<ParameterBaseHolder>()?.Parameter;
            if (targetParam != null)
            {
                int before = targetParam.CurrentHP;
                targetParam.TakeDamage(instance.Data.EffectAmount001);
                Debug.Log($"[Damage] {col.name}: {before} → {targetParam.CurrentHP}");
            }
            else
            {
                Debug.LogWarning($"[HitDetector] {col.name} に ParameterBaseHolder が未設定です。");
            }
        }
    }

    /// <summary>
    /// Sceneビューで判定範囲を可視化（Gizmos用）
    /// </summary>

}
