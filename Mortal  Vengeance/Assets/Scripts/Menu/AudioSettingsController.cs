using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider masterSlider;  //general volume
    public Slider musicSlider;  //background music 
    public Slider sfxSlider;  //effects, Ui sounds

    const string MASTER_KEY = "MasterVolume";
    const string MUSIC_KEY = "MusicVolume";
    const string SFX_KEY = "SFXVolume";

    void Start()
    {
        //Load saved settings or use defaults(0.8f)
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 0.8f);
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, 0.8f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 0.8f);

        //Set sliders to saved values(UI)
        masterSlider.value = master;
        musicSlider.value = music;
        sfxSlider.value = sfx;

        //Apply settings to audio mixer
        SetMaster(master);
        SetMusic(music);
        SetSFX(sfx);

        //Link sliders to methods(when slider is moved, call method)
        masterSlider.onValueChanged.AddListener(SetMaster);
        musicSlider.onValueChanged.AddListener(SetMusic);
        sfxSlider.onValueChanged.AddListener(SetSFX);
    }

    void SetMaster(float value)
    {
        audioMixer.SetFloat("MasterVolume", LinearToDb(value)); //Convert linear(0.0-1.0) to decibels and set mixer volume
        PlayerPrefs.SetFloat(MASTER_KEY, value); //Save value to PlayerPrefs
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
