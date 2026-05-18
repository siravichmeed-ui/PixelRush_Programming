using UnityEngine;

public class ItemDamageBoss : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (itemData.itemType == ItemType.Damage)
        {
            PlayerController player =
                other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.UseDamageItem(
                    itemData.damage
                );
            }

            gameObject.SetActive(false);

            return;
        }

        bool success =
            Inventory.Instance.Pickup(itemData);

        if (success)
        {
            gameObject.SetActive(false);
        }
    }
}