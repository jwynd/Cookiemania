using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;
using System;

public class SettingsMenu : MonoBehaviour
{
    public const string MUSIC_VOLUME = "musicVolume";
    public const string SFX_VOLUME = "sfxVolume";
    public List<GameObject> activateOnDestroy;
    public List<MonoBehaviour> enableOnDestroy;
    public AudioMixer mainMixer;

    [SerializeField]
    protected bool dontDestroy = false;


    void Awake()
    {
        Slider[] sliders = GetComponentsInChildren<Slider>();
        // 10^(mixerval / 20) = sliderval
        float musicVolume;
        float sfxVolume;
        // SetMixerFromPlayerPrefs(mainMixer);
        mainMixer.GetFloat(MUSIC_VOLUME, out musicVolume);
        mainMixer.GetFloat(SFX_VOLUME, out sfxVolume);
        sliders[0].value = Mathf.Pow(10, (musicVolume / 20));
        sliders[1].value = Mathf.Pow(10, (sfxVolume / 20));
    }

    public static void SetMixerFromPlayerPrefs(AudioMixer mainMixer)
    {
        if (PlayerPrefs.HasKey(MUSIC_VOLUME))
        {
            var volume = PlayerPrefs.GetFloat(MUSIC_VOLUME);
            mainMixer.SetFloat(MUSIC_VOLUME, volume);
        }
        if (PlayerPrefs.HasKey(SFX_VOLUME))
        {
            var volume = PlayerPrefs.GetFloat(SFX_VOLUME);
            mainMixer.SetFloat(SFX_VOLUME, volume);
        }
    }

    public void SetSFXVolume(float sfxSliderVal)
    {
        mainMixer.SetFloat(SFX_VOLUME, Mathf.Log10(sfxSliderVal) * 20);
    }

    public void SetMusicVolume(float musicSliderVal)
    {
        mainMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(musicSliderVal) * 20);
    }

    public void Return()
    {
        mainMixer.GetFloat(MUSIC_VOLUME, out var musicVolume);
        mainMixer.GetFloat(SFX_VOLUME, out var sfxVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME, sfxVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVolume);
        PlayerPrefs.Save();
        if (dontDestroy)
        {
            enabled = false;
            gameObject.SetActive(false);
            return;
        }
        Destroy(this.gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
    }

    void OnDestroy()
    {
        if (enabled)
            ReactivateObjects();
    }

    private void ReactivateObjects()
    {
        foreach (GameObject g in activateOnDestroy)
        {
            g.SetActive(true);
        }
        foreach (MonoBehaviour g in enableOnDestroy)
        {
            g.enabled = true;
        }
    }

    private void OnDisable()
    {
        ReactivateObjects();
    }
}
