using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Item Icons")]
    [SerializeField] private Image[] icons;

    [Header("Slot Backgrounds")]
    [SerializeField] private Image[] slots;

    void Update()
    {
        if (Inventory.Instance == null)
            return;

        UpdateSlots();

        UpdateSelection();
    }

    void UpdateSlots()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            ItemData item =
                Inventory.Instance.GetItem(i);

            if (item != null)
            {
                icons[i].sprite = item.icon;

                icons[i].enabled = true;
            }
            else
            {
                icons[i].enabled = false;
            }
        }
    }

    void UpdateSelection()
    {
        int selected =
            Inventory.Instance.GetSelectedIndex();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].color =
                (i == selected)
                ? Color.yellow
                : Color.white;
        }
    }
}