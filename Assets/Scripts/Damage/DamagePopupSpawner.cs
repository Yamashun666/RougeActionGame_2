using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    public GameObject popupPrefab;
    public Transform spawnPoint;

    public void ShowPopup(int value, bool isHeal = false, bool isCritical = false)
    {
        if (popupPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("[DamagePopupSpawner] prefab または spawnPoint が未設定です");
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject popup = Instantiate(popupPrefab, canvas.transform);
        popup.transform.position = Camera.main.WorldToScreenPoint(spawnPoint.position);

        DamagePopup popupComp = popup.GetComponent<DamagePopup>();
        popupComp.Initialize(value, isHeal, isCritical);
    }
}
