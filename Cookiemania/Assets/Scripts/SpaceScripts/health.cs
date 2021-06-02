using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class health : MonoBehaviour
{
    public int lives = 5;
    public int numlives = 5;

    public List<Image> hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
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

            if(i < numlives)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    public void takedamage()
    {
        lives = lives - 1;
    }

    public void gainlife()
    {
        lives = lives + 1;
    }
}
