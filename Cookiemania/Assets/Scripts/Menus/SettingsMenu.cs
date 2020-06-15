using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
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
        mainMixer.GetFloat("musicVolume", out musicVolume);
        mainMixer.GetFloat("sfxVolume", out sfxVolume);
        sliders[0].value = Mathf.Pow(10, (musicVolume / 20));
        sliders[1].value = Mathf.Pow(10, (sfxVolume / 20));
    }

    public void SetSFXVolume(float sfxSliderVal)
    {
        mainMixer.SetFloat("sfxVolume", Mathf.Log10(sfxSliderVal) * 20);
    }

    public void SetMusicVolume(float musicSliderVal)
    {
        mainMixer.SetFloat("musicVolume", Mathf.Log10(musicSliderVal) * 20);
    }

    public void Return()
    {
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
