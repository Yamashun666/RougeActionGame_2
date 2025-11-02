using System.Collections;
using UnityEngine;

public class SkillHitDetector : MonoBehaviour
{
    private int enemyLayerMask = -1;
    private GameObject hitbox;

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

    public void PerformHitDetection(SkillInstance instance, Transform origin)
    {
        GameObject hitbox = new GameObject("HitBox");
        hitbox.transform.SetParent(origin, false);
        hitbox.transform.position = origin.position;

        var collider = hitbox.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        var receiver = hitbox.AddComponent<HitboxEventReceiver>();
        receiver.Initialize(instance.Caster.GetComponent<SkillExecutor>()); // ←ここ重要！

        Destroy(hitbox, 0.3f);
        Debug.Log("[SkillHitDetector.PerformHitDetection]Called PerformHitDetection");
        Debug.Log($"[PerformHitDetection] HitBox生成完了 at {hitbox.transform.position}");

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
    }

    /// <summary>
    /// モデルの子として当たり判定を生成
    /// </summary>
    public void HitboxTransformSetter(Transform originTransform)
    {
        if (originTransform == null)
        {
            Debug.LogError("[SkillHitDetector] ModelRootが設定されていません。");
            return;
        }

        // 既にhitboxが存在しているか確認
        if (hitbox == null)
        {
            hitbox = new GameObject("HitBox");
            hitbox.transform.SetParent(originTransform, false);
            hitbox.transform.localPosition = Vector3.zero;

            // ColliderとRigidbodyを追加
            var col = hitbox.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            var rb = hitbox.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.isKinematic = true;

            hitbox.AddComponent<HitboxEventReceiver>().Initialize(GetComponent<SkillExecutor>());

            Debug.Log("[SkillHitDetector] 新規HitBoxを生成しました。");
        }
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
        hitbox.SetActive(false); // ←最初は非アクティブにしておく


        // ✅ イベント受け取りスクリプトを追加
        hitbox.AddComponent<HitboxEventReceiver>();

        Debug.Log("[HitboxGenerator] HitBoxを生成＆構成完了");

        // ★ここでHitboxEventReceiverにexecutorを渡す！
        var receiver = hitbox.AddComponent<HitboxEventReceiver>();
        receiver.executor = executor;

        Debug.Log("[HitboxGenerator] HitBox生成完了");
    }

    // 🔹 攻撃スキル発動時に呼ぶ関数
    public void ActivateHitbox(float duration)
    {
        if (hitbox == null)
        {
            Debug.LogError("[SkillHitDetector] Hitboxが存在しません。");
            return;
        }

        hitbox.SetActive(true);
        StartCoroutine(DisableAfterDelay(duration));
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox != null)
        hitbox.SetActive(false);
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
    public FactionType attackerFaction;
    private Collider2D col;

    public void Initialize(SkillExecutor owner)
    {
        executor = owner;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (executor == null)
        {
            Debug.LogWarning("[HitboxEventReceiver] executorがnullです。Initializeされていません。");
            return;
        }

        // 自分自身は無視
        if (other.gameObject == executor.gameObject)
        {
            Debug.Log("[HitboxEventReceiver] 自分自身を無視しました。");
            return;
        }

        // 攻撃者と同じFactionを弾く（ただし片方が未設定ならスキップ）
        var attackerFaction = executor.GetComponent<FactionIdentifier>()?.faction ?? FactionType.Neutral;
        var targetFaction = other.GetComponent<FactionIdentifier>()?.faction ?? FactionType.Unknown;

        if (attackerFaction != FactionType.Unknown && targetFaction != FactionType.Unknown)
        {
            if (attackerFaction == targetFaction)
            {
                Debug.Log($"[HitboxEventReceiver] 同一Faction ({attackerFaction}) のため無効化");
                return;
            }
        }

        // ParameterBaseを持つ相手のみ有効
        var targetParam = other.GetComponent<ParameterBase>();
        if (targetParam == null)
        {
            Debug.Log("[HitboxEventReceiver] ParameterBaseが見つからないため無視");
            return;
        }

        executor.OnHitEnemy(targetParam);
        Destroy(gameObject); // ヒットした瞬間にHitBoxを破壊
        Debug.Log($"[HitboxEventReceiver] {other.name} にヒットしました！");
    }


    public void PerformStepBackHit(SkillInstance instance, Transform origin)
    {
        float range = 3f;
        Vector2 dir = origin.localScale.x > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin.position, dir, range, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out Damageable dmg))
            {
                dmg.TakeDamage(instance.Data.EffectAmount001);
                Debug.Log($"[StepBackHit] {hit.collider.name} に {instance.Data.EffectAmount001} ダメージ");
            }
        }
    }

}
