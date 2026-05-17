using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public static BossUI Instance;

    [SerializeField]
    private Image fillImage;

    private int maxHP;

    void Awake()
    {
        Instance = this;

        gameObject.SetActive(false);
    }

    public void Show(int hp)
    {
        gameObject.SetActive(true);

        maxHP = hp;

        fillImage.fillAmount = 1f;
    }

    public void UpdateHP(int hp)
    {
        float percent =
            (float)hp / maxHP;

        fillImage.fillAmount =
            percent;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}