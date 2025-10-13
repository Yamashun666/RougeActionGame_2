using UnityEngine;
using Game.SkillSystem;
using System;

/// <summary>
/// スキルのヒット判定を担当する汎用クラス。
/// Transform に依存せず、呼び出し側から位置情報を渡す構造。
/// </summary>
public class SkillHitDetector
{
    /// <summary>
    /// スキルのヒット判定を実行。
    /// </summary>
    /// <param name="instance">スキルインスタンス</param>
    /// <param name="originTransform">発生元Transform（例：プレイヤー）</param>
    public void PerformHitDetection(SkillInstance instance, Transform originTransform)
    {
        if (instance == null || originTransform == null)
        {
            Debug.LogWarning("[SkillHitDetector] Invalid call: instance or originTransform is null");
            return;
        }

        Vector3 origin = originTransform.position;
        Vector3 direction = (instance.Target.Position - origin).normalized;
        float range = 3f;
        float radius = 0.5f;
        Vector3 halfExtents = Vector3.one;

        HitShape shape = instance.Data.HitShapeType;
        Collider[] hitColliders = null;

        switch (shape)
        {
            case HitShape.Box:
                hitColliders = Physics.OverlapBox(origin + direction * range / 2f, halfExtents, Quaternion.identity);
                break;
            case HitShape.Capsule:
                hitColliders = Physics.OverlapCapsule(origin, origin + direction * range, radius);
                break;
            case HitShape.Ray:
                RaycastHit hit;
                if (Physics.Raycast(origin, direction, out hit, range))
                    hitColliders = new Collider[] { hit.collider };
                else
                    hitColliders = new Collider[0];
                break;
        }

        foreach (var col in hitColliders)
        {
            ParameterBase targetParam = col.GetComponent<ParameterBaseHolder>()?.Parameter;
            if (targetParam != null)
            {
                int beforeHP = targetParam.CurrentHP;
                targetParam.TakeDamage(instance.Data.EffectAmount001);
                Debug.Log($"[Hit] {col.name} に {instance.Data.EffectAmount001} ダメージ！ {beforeHP} → {targetParam.CurrentHP}");
            }
        }
    }

    internal void PerformHitDetection(SkillInstance instance)
    {
        throw new NotImplementedException();
    }

}
