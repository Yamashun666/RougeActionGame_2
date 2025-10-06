using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int HP = 100;
    public ParameterBase parameterBase;

    public void ApplyDamage(int damage)
    {
        HP -= damage;
        Debug.Log($"{gameObject.name} が {damage} ダメージを受けた！ 残りHP: {HP}");
        if (HP <= 0) Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} は倒れた！");
        Destroy(gameObject);
    }
    public void TakeDamage(int damage)
    {
        if (parameterBase.LimitOverHP > 0)
        {
            int reduce = Mathf.Min(damage, parameterBase.LimitOverHP);
            parameterBase.LimitOverHP -= reduce;
            damage -= reduce;
        }

        if (damage > 0)
        {
            parameterBase.CurrentHP = Mathf.Max(parameterBase.CurrentHP - damage, 0);
        }
        if (parameterBase.CurrentHP <= 0)
        {
            Die();
        }
}
}
