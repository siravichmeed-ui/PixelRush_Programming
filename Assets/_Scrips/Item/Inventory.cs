using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [SerializeField] private ItemData[] items = new ItemData[3];

    private int selectedIndex = 0;

    private PlayerController player;

    // ================= SOUND =================

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip pickupSFX;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        player =
            FindObjectOfType<PlayerController>();
    }

    // ================= GET =================
    public ItemData GetItem(int index)
    {
        return items[index];
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    // ================= PICKUP =================
    public bool Pickup(ItemData item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;

                PlaySFX(pickupSFX, 1.2f);

                Debug.Log(
                    "Pickup : " + item.itemName
                );

                return true;
            }
        }

        Debug.Log("Inventory Full");

        return false;
    }

    // ================= SELECT =================
    public void SelectSlot(int index)
    {
        if (index < 0 || index >= items.Length)
            return;

        selectedIndex = index;

        Debug.Log(
            "Selected Slot : " + index
        );
    }

    // ================= USE =================
    public void UseSelectedItem()
    {
        ItemData item =
            items[selectedIndex];

        if (item == null)
            return;

        if (player != null)
        {
            switch (item.itemType)
            {
                // ================= DAMAGE =================
                case ItemType.Damage:

                    Debug.Log("USE DAMAGE ITEM");

                    player.UseDamageItem(
                        item.damage
                    );

                    break;

                // ================= HEAL =================
                case ItemType.Heal:

                    player.Heal(
                        item.healAmount
                    );

                    break;

                // ================= SPEED =================
                case ItemType.Speed:

                    player.SpeedBoost(
                        item.speedMultiplier,
                        item.speedDuration
                    );

                    break;

                // ================= IMMORTAL =================
                case ItemType.Immortal:

                    player.SetImmortal(
                        item.immortalDuration
                    );

                    break;
            }
        }
        // ลบ item หลังใช้
        items[selectedIndex] = null;

        Debug.Log("Use Item");
    }

    // ================= SOUND ================
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip, volume);
    }
}