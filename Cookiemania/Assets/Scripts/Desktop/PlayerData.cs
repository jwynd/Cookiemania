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

    public UnityEvent<int, int> WeekChanged = new UnityEvent2Int();
    public UnityEvent<int, int> MoralityChanged = new UnityEvent2Int();
    public UnityEvent<int, int> ShopLevelChanged = new UnityEvent2Int();
    public UnityEvent<int, int> MoneyChanged = new UnityEvent2Int();
    public UnityEvent<int, int> AIChanged = new UnityEvent2Int();

    //location reference
    public Tracking.PlayerLocation Location = new Tracking.PlayerLocation();
    //analytics page info
    public int race = 0;
    public int userstats = 0;
    //player data that tracks values of player money to their chosen global upgrades
    public int spacelvl = 0; //Game Dificulty
    public int incomelvl = 0;
    public int healthlvl = 0;

    //variables tracked by event system
    [SerializeField]
    private int _money = 0;
    [SerializeField]
    private int _shoplvl;
    //Game progress
    [SerializeField]
    private int _week;
    [SerializeField]
    private int _morality;
    [SerializeField]
    private int _ai;

    public int money
    {
        get { return _money; }
        set
        {
            if (_money == value) return;
            var previous = _money;
            _money = value;
            MoneyChanged?.Invoke(previous, value);
        }
    }

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
            ShopLevelChanged?.Invoke(previous, value);
        }
    }

    // range of 1 - 5
    public int difficulty { get; private set; } = 1;

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
            SetDifficulty(_week);
            WeekChanged?.Invoke(previous, value);
        }
    }

    // keep private
    private void SetDifficulty(int week)
    {
        switch (week)
        {
            case int n when n < 5:
                difficulty = 1;
                break;
            case int n when n < 21:
                difficulty = 2;
                break;
            case int n when n < 37:
                difficulty = 3;
                break;
            case int n when n < 45:
                difficulty = 4;
                break;
            default:
                difficulty = 5;
                break;
        }
    }

    public int morality
    {
        get { return _morality; }
        set
        {
            if (_morality == value) return;
            var previous = _morality;
            _morality = value;
            MoralityChanged?.Invoke(previous, value);
        }
    }

    public int ai
    {
        get { return _ai; }
        set
        {
            if (_ai == value) return;
            var previous = _ai;
            _ai = value;
            AIChanged?.Invoke(previous, value);
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
    [SerializeField]
    private int _jTimesPlayed = 0;
    [SerializeField]
    private int _jJumpPower = 0;
    // now handled by difficulty
    /*    [SerializeField]
        private int _jEnemyTypes = 0;*/
    // can only level to 1, everything after doesnt matter
    [SerializeField]
    private int _jMagnet = 0;
    // how far magnet reaches 
    [SerializeField]
    private int _jMagnetDistance = 0;
    // how quickly you can reload your net
    [SerializeField]
    private int _jMagnetCD = 0;
    [SerializeField]
    private int _jCoinJump = 0;
    [SerializeField]
    private int _jShield = 0;
    // this is no longer a field, access with Player.JUpgradeLevel
    [SerializeField]
    private int _jUpgradeLevel = 0;
    //handled in global upgrades
    /*    [SerializeField]
        private int _jHealth = 0;*/
    // the jumper AI upgrades how much value enemies have
    //handled in global upgrades
    /*    [SerializeField]
        private int _jAI = 0;*/
    // the proportion of coins/empty platforms to enemies
    // now handled by difficulty
    /*    [SerializeField]
        private int _jumperRisk = 0;*/

    // for upgrades that can increase by more than one at once
    // but can't be decreased
    private void MustIncrease(int newValue, ref int myVariable)
    {
        if (newValue <= myVariable)
            return;
        myVariable = newValue;
    }

    public int JTimesPlayed
    {
        get
        { return _jTimesPlayed; }
        // only incrementing by one allowed :)
        set
        {
            if (value == _jTimesPlayed + 1)
            {
                _jTimesPlayed = value;
            }
        }
    }

    public int JJumpPower
    {
        get { return _jJumpPower; }
        set
        {
            MustIncrease(value, ref _jJumpPower);
            JUpgradeRecalc();
        }
    }

    public int JMagnet
    {
        get { return _jMagnet; }
        set
        {
            MustIncrease(value, ref _jMagnet);
            JUpgradeRecalc();
        }
    }

    public int JMagnetDistance
    {
        get { return _jMagnetDistance; }
        set
        {
            MustIncrease(value, ref _jMagnetDistance);
            JUpgradeRecalc();
        }
    }

    public int JMagnetCD
    {
        get { return _jMagnetCD; }
        set
        {
            MustIncrease(value, ref _jMagnetCD);
            JUpgradeRecalc();
        }
    }

    public int JCoinJump
    {
        get { return _jCoinJump; }
        set
        {
            MustIncrease(value, ref _jCoinJump);
            JUpgradeRecalc();
        }
    }

    public int JShield
    {
        get { return _jShield; }
        set
        {
            MustIncrease(value, ref _jShield);
            JUpgradeRecalc();
        }
    }

    private void JUpgradeRecalc()
    {
        _jUpgradeLevel = JShield + JCoinJump + JMagnet +
            JMagnetCD + JMagnetDistance + JJumpPower;
    }

    public int JUpgradeLevel
    {
        get { return _jUpgradeLevel; }
    }

    // dict of dialogue choices made
    // first item of each list is the name of the triggered event and choice number is meaningless
    // for that one (probably gonna set to -1)
    // if there are no choices made then the list will have just the event name
    // all the choices will have the script choice number and the prompt associated with it
    public Dictionary<string, List<Tuple<string, string>>> EventChoicesMade =
       new Dictionary<string, List<Tuple<string, string>>>();

    public HashSet<string> CompletedEvents = new HashSet<string>();
    public Queue<string> QueuedEvents = new Queue<string>();
    public Dictionary<int, bool> UpgradesPurchased = new Dictionary<int, bool>();


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

        Player = this;

        // important for events
        InitGeneralVariables();
        // space game
        InitSpaceVariables();
        // jumper game, setting the private version
        // directly when necessary
        InitJumperVariables();
    }

    private void Start()
    {
        IEnumerator coroutine = StartWeekOne();
        StartCoroutine(coroutine);
    }

    private IEnumerator StartWeekOne()
    {
        // waiting a couple frames then initializing everything
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        week = 1;
    }

    private void InitGeneralVariables()
    {
        Location.Current = Parsing_Utilities.Locale.WebsiteTab;
        money = 0;
        morality = 0;
        race = 0;
        ai = 0;
        userstats = 0;
        week = 0;
    }

    // later this will init from a save file when present
    private void InitJumperVariables()
    {
        _jTimesPlayed = 0;
        _jMagnetCD = 0;
        _jMagnet = 0;
        _jMagnetDistance = 0;
        _jJumpPower = 0;
        _jShield = 0;
        _jCoinJump = 0;
        _jUpgradeLevel = 0;
    }

    private void InitSpaceVariables()
    {
        TimesPlayedSpace = 0;
        spacelvl = 0;
        incomelvl = 0;
        SpaceUpgradelvl = 0;
        healthlvl = 0;
        ShieldHealth = 0;
        GunSpread = 0;
        Pierce = 0;
        ShieldWidth = 0;
        invulnerability = 0;
    }

    [Serializable]
    public enum PlayerDataProperty
    {
        race,
        userstats,
        spacelvl,
        incomelvl,
        healthlvl,
        money,
        shoplvl,
        week,
        morality,
        ai,
        spaceupgradelvl,
        timesplayedspace,
        gunspread,
        pierce,
        shieldhealth,
        shieldwidth,
        invulnerability,
        jtimesplayed,
        jjumppower,
        jmagnet,
        jmagnetdistance,
        jmagnetcd,
        jcoinjump,
        jshield,
        jupgradelvl
    }

    // unsafe to use without ensuring Player instance != null
    private static readonly Dictionary<PlayerDataProperty, Func<int, int>> ModPlayerData =
        new Dictionary<PlayerDataProperty, Func<int, int>>
        {
           { PlayerDataProperty.ai, v => Player.ai += v },
           { PlayerDataProperty.gunspread, v => Player.GunSpread += v },
           { PlayerDataProperty.healthlvl, v => Player.healthlvl += v },
           { PlayerDataProperty.incomelvl, v => Player.incomelvl += v },
           { PlayerDataProperty.invulnerability, v => Player.invulnerability += v },
           { PlayerDataProperty.jcoinjump, v => Player.JCoinJump += v },
           { PlayerDataProperty.jjumppower, v => Player.JJumpPower += v },
           { PlayerDataProperty.jmagnet, v => Player.JMagnet += v },
           { PlayerDataProperty.jmagnetcd, v => Player.JMagnetCD += v },
           { PlayerDataProperty.jmagnetdistance, v => Player.JMagnetDistance += v },
           { PlayerDataProperty.jshield, v => Player.JShield += v },
           { PlayerDataProperty.jtimesplayed, v => Player.JTimesPlayed += v},
           // jupgrade level is read only (its auto updated)
           // { PlayerDataProperty.jupgradelvl, v => Player.JUpgradeLevel += v },
           { PlayerDataProperty.money, v => Player.money += v },
           { PlayerDataProperty.morality, v => Player.morality += v },
           { PlayerDataProperty.pierce, v => Player.Pierce += v},
           { PlayerDataProperty.race, v => Player.race += v },
           { PlayerDataProperty.shieldhealth, v => Player.ShieldHealth += v},
           { PlayerDataProperty.shieldwidth, v => Player.ShieldWidth += v},
           { PlayerDataProperty.shoplvl, v => Player.shoplvl += v},
           { PlayerDataProperty.spacelvl, v => Player.spacelvl += v},
           { PlayerDataProperty.spaceupgradelvl, v => Player.SpaceUpgradelvl += v },
           { PlayerDataProperty.timesplayedspace, v => Player.TimesPlayedSpace += v},
           { PlayerDataProperty.userstats, v => Player.userstats += v},
           { PlayerDataProperty.week, v => Player.week += v},
        };

    // returns current property level after adjustment
    public static int AddToProperty(PlayerDataProperty prop, int add)
    {
        if (Player == null) return 0;
        if (ModPlayerData.TryGetValue(prop, out Func<int, int> addFn))
        {
            return addFn.Invoke(add);
        }
        else
        {
            Debug.LogError("No function found for player data property " + prop);
            return 0;
        }
        
    }

    // only to be used in testing situations to create correct environment
    public int TestSetter(PlayerDataProperty prop, int add)
    {
        switch(prop)
        {
            case PlayerDataProperty.ai:
                _ai += add;
                return ai;
            case PlayerDataProperty.gunspread:
                GunSpread += add;
                return GunSpread;
            case PlayerDataProperty.healthlvl:
                healthlvl += add;
                return healthlvl;
            case PlayerDataProperty.incomelvl:
                incomelvl += add;
                return incomelvl;
            case PlayerDataProperty.invulnerability:
                invulnerability += add;
                return invulnerability;
            case PlayerDataProperty.jcoinjump:
                _jCoinJump += add;
                return JCoinJump;
            case PlayerDataProperty.jjumppower:
                _jJumpPower += add;
                return JJumpPower;
            case PlayerDataProperty.jmagnet:
                _jMagnet += add;
                return JMagnet;
            case PlayerDataProperty.jmagnetcd:
                _jMagnetCD += add;
                return JMagnetCD;
            case PlayerDataProperty.jmagnetdistance:
                _jMagnetDistance += add;
                return JMagnetDistance;
            case PlayerDataProperty.jshield:
                _jShield += add;
                return JShield;
            case PlayerDataProperty.jtimesplayed:
                _jTimesPlayed += add;
                return JTimesPlayed;
            case PlayerDataProperty.jupgradelvl:
                _jUpgradeLevel += add;
                return JUpgradeLevel;
            case PlayerDataProperty.money:
                _money += add;
                return money;
            case PlayerDataProperty.morality:
                _morality += add;
                return morality;
            case PlayerDataProperty.pierce:
                Pierce += add;
                return Pierce;
            case PlayerDataProperty.race:
                race += add;
                return race;
            case PlayerDataProperty.shieldhealth:
                ShieldHealth += add;
                return ShieldHealth;
            case PlayerDataProperty.shieldwidth:
                ShieldWidth += add;
                return ShieldWidth;
            case PlayerDataProperty.shoplvl:
                _shoplvl += add;
                return shoplvl;
            case PlayerDataProperty.spacelvl:
                spacelvl += add;
                return spacelvl;
            case PlayerDataProperty.spaceupgradelvl:
                SpaceUpgradelvl += add;
                return SpaceUpgradelvl;
            case PlayerDataProperty.timesplayedspace:
                TimesPlayedSpace += add;
                return TimesPlayedSpace;
            case PlayerDataProperty.userstats:
                userstats += add;
                return userstats;
            case PlayerDataProperty.week:
                week += add;
                return week;
            default: return 0;
        }
    }
}
