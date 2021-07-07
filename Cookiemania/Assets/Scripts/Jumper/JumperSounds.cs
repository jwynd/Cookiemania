using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class JumperSounds : MonoBehaviour
{
    public static JumperSounds Instance { get; private set; }

    [SerializeField]
    private AudioClip jump = null;
    [SerializeField]
    private AudioClip hit = null;
    [SerializeField]
    private AudioClip die = null;
    [SerializeField]
    private AudioClip pickup = null;
    [SerializeField]
    private AudioClip bounce = null;
    [SerializeField]
    private AudioClip platformCollapse = null;
    [SerializeField]
    private AudioClip magnet = null;
    [SerializeField]
    private AudioClip shield = null;
    [SerializeField]
    private AudioClip levelComplete = null;

    private AudioSource source;
    private bool dontPlay = false;


    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        source = GetComponent<AudioSource>();
    }

    public void Magnet()
    {
        if (dontPlay) return;
        source.PlayOneShot(magnet);
    }

    public void Shield()
    {
        if (dontPlay) return;
        source.PlayOneShot(shield);
    }

    public void Jump()
    {
        if (dontPlay) return;
        source.PlayOneShot(jump);
    }

    public void Hit()
    {
        if (dontPlay) return;
        source.PlayOneShot(hit);
    }

    public void Die()
    {
        if (dontPlay) return;
        source.clip = die;
        source.Play();
        dontPlay = true;
    }

    public void Pickup()
    {
        if (dontPlay) return;
        source.PlayOneShot(pickup);
    }

    public void Bounce()
    {
        if (dontPlay) return;
        source.PlayOneShot(bounce);
    }

    public void PlatformCollapse()
    {
        if (dontPlay) return;
        source.PlayOneShot(platformCollapse);
    }

    public void LevelComplete()
    {
        if (dontPlay) return;
        source.clip = levelComplete;
        source.Play();
        dontPlay = true;
    }

}
