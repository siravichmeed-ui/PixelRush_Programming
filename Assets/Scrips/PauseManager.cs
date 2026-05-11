using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject pauseUI;

    [SerializeField] private GameObject settingsUI;

    private bool isPaused = false;

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
    }

    void Start()
    {
        // 👉 reset game state
        Time.timeScale = 1f;

        isPaused = false;

        // 👉 ปิด UI ตอนเริ่ม
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }

        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // 👉 ถ้า settings เปิดอยู่
            // ให้ปิด settings ก่อน
            if (settingsUI != null &&
                settingsUI.activeSelf)
            {
                CloseSettings();

                return;
            }

            TogglePause();
        }
    }

    // ================= TOGGLE =================
    void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    // ================= PAUSE =================
    public void Pause()
    {
        isPaused = true;

        Time.timeScale = 0f;

        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
        }
    }

    // ================= RESUME =================
    public void Resume()
    {
        isPaused = false;

        Time.timeScale = 1f;

        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }

        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
    }

    // ================= SETTINGS =================
    public void OpenSettings()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
    }

    // ================= RESTART =================
    public void RestartGame()
    {
        // 👉 reset ทุกอย่างก่อนโหลด scene
        Time.timeScale = 1f;

        isPaused = false;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // ================= QUIT =================
    public void QuitGame()
    {
        Debug.Log("Quit");

        Application.Quit();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}