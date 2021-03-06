﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class health : MonoBehaviour
{
    public int lives = 5;
    public int numlives = 5;
    public float iframetimer = .5f;
    public float iframecurrent = 0;

    public List<Image> hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    private UnityEngine.Events.UnityAction onDeath;
    bool dead = false;
    [SerializeField] public Image heart6;
    [SerializeField] public Image heart7;
    [SerializeField] public Image heart8;
    [SerializeField] public Image heart9;
    [SerializeField] public Image heart10;

    private void Start()
    {
        if(PlayerData.Player != null)
        {
            lives += PlayerData.Player.healthlvl;
            numlives += PlayerData.Player.healthlvl;
        }
        

        switch (lives)
        {
            case 6:
                heart6.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart6);
                break;
            case 7:
                heart6.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart6);
                heart7.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart7);
                break;
            case 8:
                heart6.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart6);
                heart7.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart7);
                heart8.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart8);
                break;
            case 9:
                heart6.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart6);
                heart7.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart7);
                heart8.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart8);
                heart9.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart9);
                break;
            case 10:
                heart6.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart6);
                heart7.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart7);
                heart8.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart8);
                heart9.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart9);
                heart10.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
                hearts.Add(heart10);
                break;
        }
    }
    void Update()
    {
        if(lives > numlives)
        {
            lives = numlives;
        }

        for(int i = 0; i < hearts.Count; i++)
        {
            if(i < lives)
            {
                hearts[i].sprite = fullHeart;
            } 
            else
            {
                hearts[i].sprite = emptyHeart;
            }

          /*  if(i < numlives)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }*/

        }

        if(iframecurrent > 0)
        {
            iframecurrent -= Time.deltaTime;
        }
    }

    public void SetDeathFunction(UnityEngine.Events.UnityAction dead)
    {
        onDeath = dead;
    }
    public void takedamage()
    {
        if (dead) return;
        if(iframecurrent > 0)
        {
            return;
        }
        lives = lives - 1;
        soundmanager.Instance.PlayOneShot(soundmanager.Instance.loseheart);
        if(lives <= 0)
        {
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.playerdies);
            dead = true;
            onDeath.Invoke();
        }
        iframecurrent = iframetimer;

    }

    public void gainlife()
    {
        if (dead) return;
        lives = lives + 1;
    }
}
