using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private ParameterBase parameter;
    public Transform magicOrigin;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        parameter = GetComponent<ParameterBase>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // プレイヤーに向かって移動
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // 左右向きの切り替え
        if (direction.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }
}
