using UnityEngine;
using UnityEngine.Audio;
using static SettingsMenu;

public class SoundVolume : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer = null;
    
    void Start()
    {
        SetMixerFromPlayerPrefs(mixer);
    }
}
