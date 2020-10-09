﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    /// <summary>
    /// This class is a singleton where its a regular class that has one single static
    /// instance of the whole class. This way we are able to use the inspector to assign 
    /// public assets that can still be used anywhere like a static class even the fuctions.
    /// Reference with PlayerData.Player.x
    /// </summary>
    public static PlayerData Player;
    //player data that tracks values of player money to their chosen global upgrades
    public int spacelvl = 0; //Game Dificulty
    public int money = 0;
    public int incomelvl = 0;
    public int healthlvl = 0;
    public int shoplvl = 0; //progression in game

    //shop tier level (dependent on how many times and upgrade was selected)
    public int SpaceUpgradelvl = 0; //for shop tier management
    public int TimesPlayedSpace = 0; //A counter for times playing the space game

    //Space minigame flags go here
    public int GunSpread = 0;
    public int Pierce = 0;
    public int ShieldHealth = 0;
    public int ShieldWidth = 0;
    public int invulnerability = 0;

    //Marketing minigame flags will go here
    public int TimesPlayedMarketing = 0; //A counter for times playing the marketing game


    //Game progress
    public int month = 1; //refers to section of game vs actual month number in game. Ranges 1-5 will increase after x weeks.
    public int week = 0; // increases after every minigame
    public int morality = 0; //can go positive or negative

    //List of dialogue choices to track
    private void Awake()
    {
        if(Player != null)
        {
            GameObject.Destroy(Player);
        }
        else
        {
            Player = this;
        }
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (week > 12)
        {
            month += 1;
            week = 0;
        }
    }
}
