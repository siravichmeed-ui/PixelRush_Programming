using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public GameObject settingsPanel;
    public AudioSource audioSource;
    public AudioClip clickSound;

    public void PlaySound()
    {
        audioSource.PlayOneShot(clickSound);
        Debug.Log("Button sound .");
    }
   
}