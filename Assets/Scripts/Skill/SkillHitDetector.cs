using UnityEngine;

public class SkillHitDetector : MonoBehaviour
{
    private int enemyLayerMask = -1;
    private static GameObject hitbox;

    public SkillExecutor executor; // ★ SkillExecutor参照を保持

    [Header("位置・参照")]
    public Transform ModelRoot;

    private void Start()
    {
        // ModelRoot が未設定ならログを出して止める
        if (ModelRoot == null)
        {
            Debug.LogError("ModelRootが設定されていません。Inspectorで指定してください。");
            return;
        }
        executor = GetComponent<SkillExecutor>(); // ★ 同じGameObjectから取得
        InitializeLayerMask();
        HitboxGenerator(ModelRoot);
    }

    public void PerformHitDetection(SkillInstance instance, Transform ModelRoot)
    {
        // null チェック修正（= → ==）
        if (ModelRoot == null)
        {
            Debug.LogError("ModelRootがnullです。");
            return;
        }
        if (hitbox == null)
        {
            Debug.LogError("hitboxがnullです。");
            return;
        }

        HitboxTransformSetter(ModelRoot);
        ExecuteAttack();
    }

    /// <summary>
    /// モデルの子として当たり判定を生成
    /// </summary>
public void HitboxTransformSetter(Transform originTransform)
{
    if (originTransform == null)
    {
        Debug.LogWarning("[SkillHitDetector] originTransformがnullのためModelRootを使用します。");
        originTransform = ModelRoot;
    }

    if (hitbox == null || hitbox.Equals(null))
    {
        Debug.LogError("[SkillHitDetector] hitboxがnullのままです。生成されていません。");
        return;
    }

    // ここでデバッグログを追加
    Debug.Log($"[DEBUG] HitboxTransformSetter実行開始 - hitbox={hitbox.name} active={hitbox.activeSelf}");

    hitbox.transform.localPosition = Vector3.zero;
    hitbox.transform.localRotation = Quaternion.identity;
    hitbox.transform.localScale = new Vector3(2f, 3f, 1f);

    Rigidbody2D rb = hitbox.GetComponent<Rigidbody2D>();
    if (rb == null)
    {
        Debug.LogWarning("[SkillHitDetector] Rigidbody2D が見つからなかったので追加します。");
        rb = hitbox.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    BoxCollider2D col = hitbox.GetComponent<BoxCollider2D>();
    if (col == null)
    {
        Debug.LogWarning("[SkillHitDetector] BoxCollider2D が見つからなかったので追加します。");
        col = hitbox.AddComponent<BoxCollider2D>();
        col.size = new Vector2(2f, 3f);
        col.isTrigger = true;
    }

    Debug.Log("[DEBUG] HitboxTransformSetter完了");
}
    public void HitboxGenerator(Transform originTransform)
    {
        hitbox = new GameObject("HitBox");
        hitbox.transform.SetParent(ModelRoot, false);

        // 生成直後に基本構成を作る
        Rigidbody2D rb = hitbox.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        BoxCollider2D col = hitbox.AddComponent<BoxCollider2D>();
        col.size = new Vector2(2f, 3f);
        col.isTrigger = true;

        // ✅ イベント受け取りスクリプトを追加
        hitbox.AddComponent<HitboxEventReceiver>();

        Debug.Log("[HitboxGenerator] HitBoxを生成＆構成完了");

        // ★ここでHitboxEventReceiverにexecutorを渡す！
        var receiver = hitbox.AddComponent<HitboxEventReceiver>();
        receiver.executor = executor;

        Debug.Log("[HitboxGenerator] HitBox生成完了");
    }

    public void ExecuteAttack()
    {
        if (hitbox == null)
        {
            Debug.LogError("hitboxしばくぞ（生成されてません）");
            return;
        }
    }

    public void InitializeLayerMask()
    {
        if (enemyLayerMask == -1)
            enemyLayerMask = LayerMask.GetMask("Enemy");
    }
}

/// <summary>
/// トリガー検出用クラス（HitBoxに自動でアタッチ）
/// </summary>
public class HitboxEventReceiver : MonoBehaviour
{
    public SkillExecutor executor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("痛いンゴ");
            var parameter = other.GetComponent<ParameterBase>();
            if(executor == null)
            {
                Debug.LogError("[SkillHitDetector.OntriggerEnter2D]executorがNullです");
            }
            parameter.TakeDamage(executor.lastEffectAmount);
            Debug.Log(parameter.CurrentHP);
        }

    }
}
