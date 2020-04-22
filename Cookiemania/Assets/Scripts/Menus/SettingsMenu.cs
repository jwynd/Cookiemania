using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject[] activateOnDestroy;
    public AudioMixer mainMixer;
    // Update is called once per frame
    
    void Awake(){
        // mainMixer = Resources.Load("Master") as AudioMixer;
        // if(mainMixer == null){
        //     Debug.LogError("Audio Mixer not found");
        // }
        Slider[] sliders = GetComponentsInChildren<Slider>();
        // 10^(mixerval / 20) = sliderval
        float musicVolume;
        float sfxVolume;
        mainMixer.GetFloat("musicVolume", out musicVolume);
        mainMixer.GetFloat("sfxVolume", out sfxVolume);
        sliders[0].value = Mathf.Pow(10, (musicVolume / 20));
        sliders[1].value = Mathf.Pow(10, (sfxVolume / 20));
    }

    public void SetSFXVolume(float sfxSliderVal){
        mainMixer.SetFloat("sfxVolume", Mathf.Log10(sfxSliderVal) * 20);
    }
    
    public void SetMusicVolume(float musicSliderVal){
        mainMixer.SetFloat("musicVolume", Mathf.Log10(musicSliderVal) * 20);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            Destroy(this.gameObject);
        }
    }

    void OnDestroy(){
        foreach(GameObject g in activateOnDestroy){
            g.SetActive(true);
        }
    }
}
