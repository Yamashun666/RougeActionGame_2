using System.Collections;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    private SkillData skillData;
    private ParameterBase caster;
    private Rigidbody2D rb;
    private float lifeTimer;

    public void Initialize(SkillData data, ParameterBase caster, Vector2 direction)
    {
        skillData = data;
        this.caster = caster;

        rb = GetComponent<Rigidbody2D>();
        float speed = data.EffectAmount002 > 0 ? data.EffectAmount002 : 10f;

        // ✅ ここを修正：directionベクトルをそのまま使う
        rb.linearVelocity = direction.normalized * speed;

        float lifetime = data.EffectAmount003 > 0 ? data.EffectAmount003 : 3f;
        StartCoroutine(LifeRoutine(lifetime)); // ✅ コルーチンで寿命管理
        Debug.Log("[MagicProjectile]Called Initialize");
        Debug.Log("[MagicProjectile]弾の寿命は" + lifetime + "秒です。");
    }

    private IEnumerator LifeRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            ParameterBase enemyParam = col.GetComponent<ParameterBase>();
            if (enemyParam != null)
            {
                int damage = skillData.EffectAmount001;
                enemyParam.TakeDamage(damage);
                Debug.Log($"[MagicProjectile] {skillData.SkillName} hit! damage={damage}");
            }

            // VFXやヒット効果再生もここで
            Destroy(gameObject);
        }
    }
}
