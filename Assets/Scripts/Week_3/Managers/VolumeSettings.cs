using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    private const float MinimumVolume = 0.0001f;

    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masterSlider;

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(_ => SetMusicVolume());
        sfxSlider.onValueChanged.AddListener(_ => SetSFXVolume());
        masterSlider.onValueChanged.AddListener(_ => SetMasterVolume());
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }else{
            SetMusicVolume();
            SetSFXVolume();
            SetMasterVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", ConvertToDecibels(volume));
        PlayerPrefs.SetFloat("musicVolume", volume);
        Debug.Log("Music Volume: " + volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        myMixer.SetFloat("SFX", ConvertToDecibels(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);
        Debug.Log("SFX Volume: " + volume);
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        myMixer.SetFloat("master", ConvertToDecibels(volume));
        PlayerPrefs.SetFloat("masterVolume", volume);
        Debug.Log("Master Volume: " + volume);
    }

    private static float ConvertToDecibels(float volume)
    {
        return volume <= 0f ? -80f : Mathf.Log10(Mathf.Max(volume, MinimumVolume)) * 20f;
    }
    
    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMusicVolume();
        SetSFXVolume();
        SetMasterVolume();
    }
}
