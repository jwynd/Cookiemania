using UnityEngine;
using static Parsing_Utilities;
using static Tracking.LocationUtils;

public class MusicSwapper : MonoBehaviour
{
    [SerializeField]
    private AudioClip desktopMusic = null;
    [SerializeField]
    private AudioClip jumperMusic = null;
    [SerializeField]
    private AudioClip spaceMusic = null;

    private AudioSource audioSource;

    private void ChangeSong(Locale previous, Locale current)
    {
        if (current.IsDesktop())
        {
            CheckToChange(desktopMusic);
        }
        else if (current == Locale.JumpingMinigame)
        {
            CheckToChange(jumperMusic);
        }
        else if (current == Locale.SpaceMinigame)
        {
            CheckToChange(spaceMusic);
        }
        
    }

    private void CheckToChange(AudioClip clip)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayerData.Player.Location.Updated.AddListener(ChangeSong);
        CheckToChange(desktopMusic);
    }
}
