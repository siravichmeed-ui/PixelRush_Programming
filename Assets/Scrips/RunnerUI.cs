using UnityEngine;
using System.Collections;
using TMPro;

public class RunnerUI : MonoBehaviour
{
    [Header("UI")]
    public RectTransform playerIcon;

    public RectTransform line;

    [Header("Distance Text")]
    public TextMeshProUGUI distanceText;

    [Header("Distance")]
    public float maxDistance = 300f;

    private bool endlessStarted = false;

    // 👉 เก็บระยะตอนเริ่ม endless
    private float endlessStartDistance = 0f;

    IEnumerator Start()
    {
        yield return null;

        ResetUI();
    }

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (Time.timeScale == 0f)
            return;

        MovePlayer();

        UpdateDistanceText();
    }

    void MovePlayer()
    {
        float distance =
            GameManager.Instance.distance;

        float displayDistance;

        // ================= BEFORE ENDLESS =================
        if (!GameManager.Instance.isEndlessPhase)
        {
            // 👉 ค้างสุดเส้นตอน boss
            displayDistance =
                Mathf.Clamp(
                    distance,
                    0f,
                    maxDistance
                );
        }
        // ================= ENDLESS =================
        else
        {
            // 👉 เริ่ม endless ครั้งแรก
            if (!endlessStarted)
            {
                endlessStarted = true;

                // 👉 จำระยะตอน boss ตาย
                endlessStartDistance =
                    distance;

                ResetUI();
            }

            // 👉 เริ่มนับใหม่จาก 0
            float endlessDistance =
                distance -
                endlessStartDistance;

            // 👉 วนทุก 300m
            displayDistance =
                endlessDistance %
                maxDistance;
        }

        float t =
            Mathf.Clamp01(
                displayDistance /
                maxDistance
            );

        float width =
            line.rect.width;

        Vector2 pos =
            playerIcon.anchoredPosition;

        pos.x = t * width;

        playerIcon.anchoredPosition =
            pos;
    }

    // ================= DISTANCE TEXT =================
    void UpdateDistanceText()
    {
        if (distanceText == null)
            return;

        int meter =
            Mathf.FloorToInt(
                GameManager.Instance.distance
            );

        distanceText.text =
            meter + " m";
    }

    public void ResetUI()
    {
        Vector2 pos =
            playerIcon.anchoredPosition;

        pos.x = 0f;

        playerIcon.anchoredPosition =
            pos;
    }
}