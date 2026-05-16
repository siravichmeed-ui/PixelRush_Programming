using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(int hp)
    {
        gameObject.SetActive(true);

        slider.maxValue = hp;

        slider.value = hp;
    }

    public void UpdateHP(int hp)
    {
        slider.value = hp;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}