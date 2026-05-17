//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class UIManager : MonoBehaviour
//{
//    public GameObject popupUI;

//    public void ShowUI()
//    {
//        popupUI.SetActive(true);
//    }

//    public void HideUI()
//    {
//        popupUI.SetActive(false);
//    }

//    public void Quit()
//    {
//        Application.Quit();
//    }

//    public void Game()
//    {
//        SceneManager.LoadScene(1);
//    }

//    public void Home()
//    {
//        SceneManager.LoadScene(0);
//    }

//    // 🔥 เพิ่มอันนี้
//    public void Restart()
//    {
//        GameManager.Instance.RestartGame();
//    }
//}

using System.Collections; // 🔥 จำเป็นต้องเพิ่มอันนี้เข้ามาเพื่อใช้ IEnumerator
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject popupUI;

    [Header("Cutscene Settings")]
    [SerializeField] private CanvasGroup howToPlayPanel; // 🔥 ลาก Panel วิธีเล่นที่มี Canvas Group มาใส่ตรงนี้
    [SerializeField] private float waitTime = 5f;        // เวลาหยุดรอ (5 วินาที)
    [SerializeField] private float fadeDuration = 1f;    // เวลาที่ใช้ในการจางหาย (1 วินาที)

    private void Start()
    {
        // 🔥 ตรวจสอบว่าถ้าอยู่ใน Scene เกม (เช่น SceneIndex 1) และมีการใส่ Panel วิธีเล่นไว้ ให้เริ่มเล่น Cutscene ทันที
        if (SceneManager.GetActiveScene().buildIndex == 1 && howToPlayPanel != null)
        {
            StartCoroutine(PlayCutsceneRoutine());
        }
    }

    // 🔥 Coroutine ควบคุมการหน่วงเวลา 5 วิ และค่อยๆ จางหาย
    private IEnumerator PlayCutsceneRoutine()
    {
        // 1. สั่งหยุดเวลาในเกมทั้งหมดทันที (ทุกอย่างจะหยุดนิ่ง รอผู้เล่น)
        Time.timeScale = 0f;

        // เปิดใช้งาน Panel (เผื่อลืมเปิดทิ้งไว้ใน Editor)
        howToPlayPanel.gameObject.SetActive(true);
        howToPlayPanel.alpha = 1f;

        // 2. หยุดรอเป็นเวลา 5 วินาทีจริง (ต้องใช้ Realtime เพราะเราสั่ง Time.timeScale = 0 ไป)
        yield return new WaitForSecondsRealtime(waitTime);

        // 3. ค่อยๆ จางหายไป (Fade Out)
        float counter = 0f;
        while (counter < fadeDuration)
        {
            // ใช้ unscaledDeltaTime เพราะเวลาหลักของเกมยังถูกหยุดอยู่
            counter += Time.unscaledDeltaTime;
            howToPlayPanel.alpha = Mathf.Lerp(1f, 0f, counter / fadeDuration);
            yield return null;
        }

        // ปิดให้สนิทและปิดตัว Object ไปเลยเพื่อไม่ให้บล็อกการคลิกเมาส์ในเกม
        howToPlayPanel.alpha = 0f;
        howToPlayPanel.gameObject.SetActive(false);

        // 4. ปล่อยเวลาให้เกมเดินตามปกติ เริ่มเกมได้!
        Time.timeScale = 1f;
    }

    public void ShowUI()
    {
        popupUI.SetActive(true);
    }

    public void HideUI()
    {
        popupUI.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Game()
    {
        SceneManager.LoadScene(1);
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        // ก่อนโหลด Scene ใหม่หรือ Reset มั่นใจว่าเปิดเวลาให้เป็นปกติก่อน
        Time.timeScale = 1f;
        GameManager.Instance.RestartGame();
    }
}