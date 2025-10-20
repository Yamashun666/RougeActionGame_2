using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private string defaultSceneName = "LobbyScene"; // ← Inspectorから設定
    [SerializeField] private Button button;

    private bool isClicked = false;
    private static SceneChange instance;
    public static SceneChange Instance => instance;

    private void Awake()
    {
        // シングルトン（どのシーンでも呼べるように）
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(() => ChangeScene(defaultSceneName));
    }

    /// <summary>
    /// 外部からも呼び出せる共通関数
    /// </summary>
    public void ChangeScene(string sceneName = null)
    {
        if (isClicked)
        {
            Debug.Log("[SceneChange] シーン遷移中（二重呼び出し防止）");
            return;
        }

        string targetScene = string.IsNullOrEmpty(sceneName) ? defaultSceneName : sceneName;

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("[SceneChange] 遷移先シーン名が設定されていません。");
            return;
        }

        isClicked = true;
        Debug.Log($"[SceneChange] シーン '{targetScene}' に遷移します。");
        SceneManager.LoadScene(targetScene);
    }
}
