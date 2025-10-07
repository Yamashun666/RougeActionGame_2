using TMPro;
using UnityEngine;

/// <summary>
/// 所持コイン数をUIにリアルタイム表示
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("参照")]
    public PlayerInventory playerInventory;
    public TMP_Text coinText;

    private void Start()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();

        if (playerInventory != null)
            playerInventory.OnCoinChanged += UpdateCoinUI;

        UpdateCoinUI(playerInventory?.coinCount ?? 0);
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnCoinChanged -= UpdateCoinUI;
    }

    private void UpdateCoinUI(int newCount)
    {
        if (coinText != null)
            coinText.text = $"Coins: {newCount}";
    }
}
