using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [Header("設定")]
    public string itemName = "Coin";
    public int value = 1;
    public float lifeTime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup(other.gameObject);
        }
    }

    private void Pickup(GameObject player)
    {
        var inv = player.GetComponent<PlayerInventory>();
        if (inv != null)
        {
            inv.AddItem(itemName, value);
        }

        Debug.Log($"💰 {player.name} が {itemName} (x{value}) を拾った！");
        Destroy(gameObject);
    }
}
