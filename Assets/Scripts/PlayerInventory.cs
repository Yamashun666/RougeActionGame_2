using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    public int coinCount = 0;
    public event Action<int> OnCoinChanged; // UI更新イベント

    public void AddItem(string itemName, int value)
    {
        if (itemName == "Coin")
        {
            coinCount += value;
            OnCoinChanged?.Invoke(coinCount); // UI通知
        }

        Debug.Log($"🧍 コイン獲得！ 合計: {coinCount}");
    }
}
