using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public class UnityEvent2Int : UnityEvent<int, int>
    {

    }

    public UnityEvent<int, int> OnWeekChanged = new UnityEvent2Int();
    public UnityEvent<int, int> OnMoralityChanged = new UnityEvent2Int();
    public UnityEvent<int, int> OnShopLvlChanged = new UnityEvent2Int();
    public UnityEvent<int, int> OnMoneyChanged = new UnityEvent2Int();

    //player data that tracks values of player money to their chosen global upgrades
    public int spacelvl = 0; //Game Dificulty
    public int incomelvl = 0;
    public int healthlvl = 0;
    [SerializeField]
    private int _money = 0;
    public int money
    {
        get { return _money; }
        set
        {
            if (_money == value) return;
            var previous = _money;
            _money = value;
            OnMoneyChanged?.Invoke(previous, value);
        }
    }
    [SerializeField]
    private int _shoplvl;
    public int shoplvl //Follows progression of player and shop access
    {
        get { return _shoplvl; }
        set
        {
            if (value <= _shoplvl)
            {
                return;
            }
            var previous = _shoplvl;
            _shoplvl = value;
            OnShopLvlChanged?.Invoke(previous, value);
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
    [SerializeField]
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
            var previous = _week;
            _week = value;
            OnWeekChanged?.Invoke(previous, value);
        }
    }

    [SerializeField]
    private int _morality;
    public int morality
    {
        get { return _morality; }
        set
        {
            if (_morality == value) return;
            var previous = _morality;
            _morality = value;
            OnMoralityChanged?.Invoke(previous, value);
        }
    }

    // dict of dialogue choices made
    // first item of each list is the name of the triggered event and choice number is meaningless
    // for that one (probably gonna set to -1)
    // if there are no choices made then the list will have just the event name
    // all the choices will have the script choice number and the prompt associated with it
    public Dictionary<string, List<Tuple<string, string>>> EventChoicesMade =
       new Dictionary<string, List<Tuple<string, string>>>();

    public void PrintChoicesMade()
    {
        foreach (var @event in EventChoicesMade.Keys)
        {
            Debug.Log(@event + " choices made: "
                + string.Join(", ", EventChoicesMade[@event]));
        }
    }

    // increment specifically for buttons
    public void IncrementWeek()
    {
        week += 1;
    }

    public void IncrementShopLvl()
    {
        shoplvl += 1;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void AddMorality(int amount)
    {
        morality += amount;
    }

    private void Awake()
    {
        if (Player != null && Player != this)
        {
            Destroy(Player);
            return;
        }
        else
        {
            Player = this;
        }
    }
}
