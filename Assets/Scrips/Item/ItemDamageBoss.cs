using UnityEngine;

public class ItemDamageBoss : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        bool success =
            Inventory.Instance.Pickup(itemData);

        if (success)
        {
            gameObject.SetActive(false);
        }
    }
}