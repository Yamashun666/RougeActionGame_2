using UnityEngine;
using Game.SkillSystem;

/// <summary>
/// スキルの攻撃判定を行うクラス。
/// 判定形状：Box / Capsule / Ray に対応。
/// </summary>
public class SkillHitDetector
{
    public void PerformHitDetection(SkillInstance instance)
    {
        Vector3 origin = instance.Caster.Position;
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
                hitColliders = PerformRaycast(origin, direction, range);
                break;
        }

        foreach (var col in hitColliders)
        {
            ParameterBase targetParam = col.GetComponent<ParameterBaseHolder>()?.Parameter;
            if (targetParam != null)
            {
                targetParam.TakeDamage(instance.Data.EffectAmount001);
                Debug.Log($"Hit {col.name}, Damage: {instance.Data.EffectAmount001}");
            }
        }
    }

    private Collider[] PerformRaycast(Vector3 origin, Vector3 direction, float range)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, range))
            return new Collider[] { hit.collider };
        return new Collider[0];
    }
}
