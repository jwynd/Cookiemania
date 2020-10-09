using System;
using System.Collections;
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
    private int _money = 0;
    public int incomelvl = 0;
    public int healthlvl = 0;
    public int shoplvl = 0; //progression in game
    private int _shoplvl = 0;

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
    public int week { get { return week; }
    set
        {
            if (value <= week)
            {
                return;
            }
            week = value;
            //OnWeekChanged(this, week);
        }
    }
    private int _week = 0;
    public int morality = 0; //can go positive or negative
    private int _morality = 0;

    //List of dialogue choices to track

    //Event stuff
    public event EventHandler OnWeekChanged;
    public event EventHandler OnMoralityChanged;
    public event EventHandler OnShopLvlChanged;
    public event EventHandler OnMoneyChanged;
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
        if(_week != week)
        {
            _week = week;
            if (OnWeekChanged != null) OnWeekChanged(this, EventArgs.Empty);
        }
        if(_morality != morality)
        {
            _morality = morality;
            if (OnMoralityChanged != null) OnMoralityChanged(this, EventArgs.Empty);
        }
        if(_money != money)
        {
            _money = money;
            if (OnMoneyChanged != null) OnMoneyChanged(this, EventArgs.Empty);
        }
        if(_shoplvl != shoplvl)
        {
            if (OnShopLvlChanged != null) OnShopLvlChanged(this, EventArgs.Empty);
        }
        
    }
}
