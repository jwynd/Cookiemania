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
    private int _money = 0;
    public int money
    {
        get { return _money; }
        set
        {
            _money = value;
            OnMoneyChanged?.Invoke(this, new IntegerEventArgs(value));
        }
    }
    private int _shoplvl;
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
            OnShopLvlChanged?.Invoke(this, new IntegerEventArgs(value));
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
    private int _week;
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
            OnWeekChanged?.Invoke(this, new IntegerEventArgs(value));
        }
    }

    private int _morality;
    public int morality
    {
        get { return _morality; }
        set
        {
            _morality = value;
            OnMoralityChanged?.Invoke(this, new IntegerEventArgs(value));
        }
    }

    // List of dialogue choices made
    // first item of each list is the name of the triggered event and choice number is meaningless
    // for that one (probably gonna set to -1)
    // if there are no choices made then the list will have just the event name
    // all the choices will have the script choice number and the dialogue line associated with it
    public List<List<Tuple<int, string>>> EventChoicesMade = new List<List<Tuple<int, string>>>();
    private void Awake()
    {
        if (Player != null && Player != this)
        {
            Destroy(Player);
        }
        else
        {
            Player = this;
        }
    }
}
