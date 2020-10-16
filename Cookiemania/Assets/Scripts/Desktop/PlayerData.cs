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
    //Event stuff
    public event EventHandler OnWeekChanged;
    public event EventHandler OnMoralityChanged;
    public event EventHandler OnShopLvlChanged;
    public event EventHandler OnMoneyChanged;

    public class IntegerEventArgs : EventArgs
    {
        public int Amount { get; private set; } = 0;
        public IntegerEventArgs(int amt)
        {
            Amount = amt;
        }
    }

    //player data that tracks values of player money to their chosen global upgrades
    public int spacelvl = 0; //Game Dificulty
    public int incomelvl = 0;
    public int healthlvl = 0;
    public int _money = 0;
    public int money
    {
        get { return _money; }
        set
        {
            _money = value;
            OnMoneyChanged(this, new IntegerEventArgs(_money));
        }
    }
    public int _shoplvl;
    public int shoplvl //Follows progression of player and shop access
    {
        get { return _shoplvl; }
        set
        {
            if(value <= _shoplvl)
            {
                return;
            }
            _shoplvl = value;
            OnShopLvlChanged(this, new IntegerEventArgs(_shoplvl));
        }
    }

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
    public int _week;
    public int week
    {
        get { return _week; }
        set
        {
            if (value <= _week)
            {
                return;
            }
            _week = value;
            OnWeekChanged(this, new IntegerEventArgs(_week));
        }
    }

    public int _morality;
    public int morality
    {
        get { return _morality; }
        set
        {
            _morality = value;
            OnMoralityChanged(this, new IntegerEventArgs(_morality));
        }
    }

    //List of dialogue choices to track

    private void Awake()
    {
        if (Player != null)
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

    }
}
