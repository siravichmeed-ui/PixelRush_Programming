using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Distance")]
    public float distance = 0f;

    [Header("Speed")]
    public float baseSpeed = 5f;

    public float speed;

    public float speedIncreaseRate = 0.02f;

    [Header("State")]
    public bool isGameRunning = true;

    public bool isBossPhase = false;

    public bool isEndlessPhase = false;

    void Awake()
    {
        Time.timeScale = 1f;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);

            return;
        }

        speed = baseSpeed;
    }

    void Update()
    {
        if (!isGameRunning)
            return;

        distance += speed * Time.deltaTime;

        // 👉 เพิ่มความเร็วเรื่อย ๆ
        speed +=
            speedIncreaseRate *
            Time.deltaTime;
    }

    // ================= CONTROL =================
    public void StopGame()
    {
        isGameRunning = false;

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isGameRunning = true;

        Time.timeScale = 1f;
    }

    // ================= RESET =================
    public void ResetGame()
    {
        distance = 0f;

        speed = baseSpeed;

        isBossPhase = false;

        isEndlessPhase = false;

        isGameRunning = true;

        Time.timeScale = 1f;
    }

    // ================= BOSS =================
    public void EnterBossPhase()
    {
        isBossPhase = true;

        speedIncreaseRate = 0f;
    }

    // ================= ENDLESS =================
    public void BossDefeated()
    {
        Debug.Log("ENTER ENDLESS MODE");

        isBossPhase = false;

        isEndlessPhase = true;

        // 👉 เพิ่มความเร็วอีกครั้ง
        speedIncreaseRate = 0.05f;
    }

    // ================= RESTART =================
    public void RestartGame()
    {
        Time.timeScale = 1f;

        ResetGame();

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}