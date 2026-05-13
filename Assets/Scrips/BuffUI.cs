using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffUI : MonoBehaviour
{
    // ================= SPEED =================
    [Header("Speed")]
    [SerializeField]
    private GameObject speedUI;

    [SerializeField]
    private TMP_Text speedText;

    [SerializeField]
    private Image speedIcon;

    [SerializeField]
    private Sprite speedSprite;

    // ================= IMMORTAL =================
    [Header("Immortal")]
    [SerializeField]
    private GameObject immortalUI;

    [SerializeField]
    private TMP_Text immortalText;

    [SerializeField]
    private Image immortalIcon;

    [SerializeField]
    private Sprite immortalSprite;

    void Start()
    {
        // 👉 ปิด UI ตอนเริ่ม
        if (speedUI != null)
        {
            speedUI.SetActive(false);
        }

        if (immortalUI != null)
        {
            immortalUI.SetActive(false);
        }

        // 👉 ใส่รูป speed
        if (
            speedIcon != null &&
            speedSprite != null
        )
        {
            speedIcon.sprite =
                speedSprite;
        }

        // 👉 ใส่รูป immortal
        if (
            immortalIcon != null &&
            immortalSprite != null
        )
        {
            immortalIcon.sprite =
                immortalSprite;
        }
    }

    // ================= SPEED =================
    public void UpdateSpeed(
        float time
    )
    {
        if (time > 0f)
        {
            if (speedUI != null)
            {
                speedUI.SetActive(true);
            }

            if (speedText != null)
            {
                speedText.text =
                    time.ToString("F1") + "s";
            }
        }
        else
        {
            if (speedUI != null)
            {
                speedUI.SetActive(false);
            }
        }
    }

    // ================= IMMORTAL =================
    public void UpdateImmortal(
        float time
    )
    {
        if (time > 0f)
        {
            if (immortalUI != null)
            {
                immortalUI.SetActive(true);
            }

            if (immortalText != null)
            {
                immortalText.text =
                    time.ToString("F1") + "s";
            }
        }
        else
        {
            if (immortalUI != null)
            {
                immortalUI.SetActive(false);
            }
        }
    }
}