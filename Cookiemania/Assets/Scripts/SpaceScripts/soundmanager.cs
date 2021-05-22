using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundmanager : MonoBehaviour
{
    // Holds the single instance of the SoundManager that 
    // you can access from any script
    public static soundmanager Instance = null;

    // All sound effects in the game
    public AudioClip enemyfire;
    public AudioClip playerfire;
    public AudioClip enemydies;
    public AudioClip playerdies;
    public AudioClip loseheart;
    public AudioClip shield;
    public AudioClip gainlife;

    //continue for audio files

    private AudioSource soundEffectAudio;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }

        AudioSource theSource = GetComponent<AudioSource>();
        soundEffectAudio = theSource;
    }

    // Other GameObjects can call this to play sounds// Other GameObjects can call this to play sounds
    public void PlayOneShot(AudioClip clip)
    {
        soundEffectAudio.PlayOneShot(clip);
    }
 
}
