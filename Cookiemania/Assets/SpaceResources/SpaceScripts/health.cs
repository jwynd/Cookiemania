using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class health : MonoBehaviour
{
    public int lives;
    public int numlives;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Update()
    {
        if(lives > numlives)
        {
            lives = numlives;
        }

        for(int i = 0; i < hearts.Length; i++)
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
