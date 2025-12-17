using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    const string MASTER_KEY = "MasterVolume";
    const string MUSIC_KEY = "MusicVolume";
    const string SFX_KEY = "SFXVolume";

    void Start()
    {
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 0.8f);
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, 0.8f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 0.8f);

        masterSlider.value = master;
        musicSlider.value = music;
        sfxSlider.value = sfx;

        SetMaster(master);
        SetMusic(music);
        SetSFX(sfx);

        masterSlider.onValueChanged.AddListener(SetMaster);
        musicSlider.onValueChanged.AddListener(SetMusic);
        sfxSlider.onValueChanged.AddListener(SetSFX);
    }

    void SetMaster(float value)
    {
        audioMixer.SetFloat("MasterVolume", LinearToDb(value));
        PlayerPrefs.SetFloat(MASTER_KEY, value);
    }

    void SetMusic(float value)
    {
        audioMixer.SetFloat("MusicVolume", LinearToDb(value));
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    void SetSFX(float value)
    {
        audioMixer.SetFloat("SFXVolume", LinearToDb(value));
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    float LinearToDb(float value)
    {
        return value <= 0.001f ? -80f : Mathf.Log10(value) * 20f;
    }
}
