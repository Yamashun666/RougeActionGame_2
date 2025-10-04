using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float HP = 100f;

    public void ApplyDamage(float damage)
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
}
