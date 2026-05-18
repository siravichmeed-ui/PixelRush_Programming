using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI distanceText;

    void OnEnable()
    {
        ShowDistance();
    }

    void ShowDistance()
    {
        if (GameManager.Instance == null)
            return;

        int distance =
            Mathf.FloorToInt(
                GameManager.Instance.distance
            );

        distanceText.text =
            "Distance : " +
            distance +
            " M";
    }
}