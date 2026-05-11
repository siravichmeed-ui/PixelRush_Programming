using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Distance")]
    public float distance = 0f;

    [Header("Speed")]
    public float speed = 5f;

    public float speedIncreaseRate = 0.02f;

    [Header("State")]
    public bool isGameRunning = true;

    public bool isBossPhase = false;

    void Awake()
    {
        // ================= SINGLETON =================
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);

            return;
        }

        ResetGame();
    }

    void Update()
    {
        if (!isGameRunning)
            return;

        distance +=
            speed * Time.deltaTime;

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

        speed = 5f;

        speedIncreaseRate = 0.02f;

        isBossPhase = false;

        isGameRunning = true;

        Time.timeScale = 1f;
    }

    // ================= BOSS =================
    public void EnterBossPhase()
    {
        isBossPhase = true;

        speedIncreaseRate = 0f;
    }

    public void BossDefeated()
    {
        Debug.Log("Boss Cleared!");

        StopGame();
    }

    // ================= RESTART =================
    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}