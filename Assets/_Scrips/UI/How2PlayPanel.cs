using System.Collections;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup howToPlayPanel; 
    [SerializeField] private float waitTime = 5f;       
    [SerializeField] private float fadeDuration = 1f;   

    void Start()
    {
        if (howToPlayPanel != null)
        {
            StartCoroutine(StartGameRoutine());
        }
    }

    private IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(waitTime);

        float counter = 0f;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            howToPlayPanel.alpha = Mathf.Lerp(1f, 0f, counter / fadeDuration);
            yield return null;
        }

        howToPlayPanel.alpha = 0f;
        howToPlayPanel.gameObject.SetActive(false);

    }
}