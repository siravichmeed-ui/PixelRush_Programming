using UnityEngine;

public enum ItemType
{
    Damage,
    Speed,
    Immortal,
    Heal
}

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;

    public Sprite icon;

    public ItemType itemType;

    // ================= DAMAGE =================
    [Header("Damage")]
    public int damage = 1;

    // ================= HEAL =================
    [Header("Heal")]
    public int healAmount = 1;

    // ================= SPEED =================
    [Header("Speed")]
    public float speedMultiplier = 2f;

    public float speedDuration = 5f;

    // ================= IMMORTAL =================
    [Header("Immortal")]
    public float immortalDuration = 5f;
}