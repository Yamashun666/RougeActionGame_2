using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class SceneChange : MonoBehaviour
{
    private Button button;
    private bool isClicked = false;
    [SerializeField] public string changeSceneName;
    void Start()
    {

        button = GetComponent<Button>();
    }
    void Update()
    {
        if (isClicked)
        {
            Debug.Log("[SceneChange.OnButtonClicked] すでにシーン遷移ボタンが押されています。");
            return; // 二重押し防止
        }

        isClicked = true;

        if (!string.IsNullOrEmpty(changeSceneName))
        {
            SceneManager.LoadScene(changeSceneName);
        }
        else
        {
            Debug.LogError("[SceneChange] 遷移先シーン名が設定されていません。");
            isClicked = false; // エラー時は戻す
        }

    }

}
