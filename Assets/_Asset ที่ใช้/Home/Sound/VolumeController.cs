using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider musicSlider;


    public void SetVolume()
    {
        float volume = musicSlider.value;
        mixer.SetFloat("Home", Mathf.Log10(volume) * 20);
    }
}