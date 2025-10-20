using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private string changeSceneName;
    private Button button;
    private bool isClicked = false;

    void Start()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("[SceneChange] Button コンポーネントが見つかりません。");
            return;
        }

        // ✅ クリックイベントを登録
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (isClicked)
        {
            Debug.Log("[SceneChange] 二重クリックを防止しました。");
            return;
        }

        isClicked = true;

        if (string.IsNullOrEmpty(changeSceneName))
        {
            Debug.LogError("[SceneChange] 遷移先シーン名が設定されていません。");
            isClicked = false;
            return;
        }

        Debug.Log($"[SceneChange] シーン '{changeSceneName}' に遷移します。");
        SceneManager.LoadScene(changeSceneName);
    }
}
