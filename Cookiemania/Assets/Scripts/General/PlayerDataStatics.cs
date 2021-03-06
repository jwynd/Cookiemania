﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataStatics
{
    public static string SAVE_EXTENSION = ".SAV";
    public static string SAVE_FOLDER = "/SAV/";
    // if this key exists try to load a save from it
    public static string P_PREFS_LOAD = "LoadGame";
    // this key is the "continue" option
    // when this continue is selected LoadGame should also be set to this same value 
    // so playerdata doesn't have to do anything different
    public static string P_PREFS_LAST_SAVED = "LastSavedGame";
    public static string P_PREFS_SLOT_1 = "Save1";
    public static string P_PREFS_SLOT_2 = "Save2";
    public static string P_PREFS_SLOT_3 = "Save3";
    public static string P_PREFS_SLOT_1_NAME = "SlotOneName";
    public static string P_PREFS_SLOT_2_NAME = "SlotTwoName";
    public static string P_PREFS_SLOT_3_NAME = "SlotThreeName";
    public static string P_PREFS_SLOT_1_MONEY = "SlotOneMoney";
    public static string P_PREFS_SLOT_2_MONEY = "SlotTwoMoney";
    public static string P_PREFS_SLOT_3_MONEY = "SlotThreeMoney";

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

    [Serializable]
    public enum LoadState
    {
        NewGame,
        LoadSuccess,
        LoadFail
    }

    // unsafe to use without ensuring Player instance != null
    private static readonly Dictionary<PlayerDataProperty, Func<int, int>> ModPlayerData =
        new Dictionary<PlayerDataProperty, Func<int, int>>
        {
           { PlayerDataProperty.ai, v => PlayerData.Player.ai += v },
           { PlayerDataProperty.gunspread, v => PlayerData.Player.GunSpread += v },
           { PlayerDataProperty.healthlvl, v => PlayerData.Player.healthlvl += v },
           { PlayerDataProperty.incomelvl, v => PlayerData.Player.incomelvl += v },
           { PlayerDataProperty.invulnerability, v => PlayerData.Player.invulnerability += v },
           { PlayerDataProperty.jcoinjump, v => PlayerData.Player.JCoinJump += v },
           { PlayerDataProperty.jjumppower, v => PlayerData.Player.JJumpPower += v },
           { PlayerDataProperty.jmagnet, v => PlayerData.Player.JMagnet += v },
           { PlayerDataProperty.jmagnetcd, v => PlayerData.Player.JMagnetCD += v },
           { PlayerDataProperty.jmagnetdistance, v => PlayerData.Player.JMagnetDistance += v },
           { PlayerDataProperty.jshield, v => PlayerData.Player.JShield += v },
           { PlayerDataProperty.jtimesplayed, v => PlayerData.Player.JTimesPlayed += v},
           // jupgrade level is read only (its auto updated)
           { PlayerDataProperty.jupgradelvl, v => PlayerData.Player.JUpgradeLevel },
           { PlayerDataProperty.money, v => PlayerData.Player.money += v },
           { PlayerDataProperty.morality, v => PlayerData.Player.morality += v },
           { PlayerDataProperty.pierce, v => PlayerData.Player.Pierce += v},
           { PlayerDataProperty.race, v => PlayerData.Player.race += v },
           { PlayerDataProperty.shieldhealth, v => PlayerData.Player.ShieldHealth += v},
           { PlayerDataProperty.shieldwidth, v => PlayerData.Player.ShieldWidth += v},
           { PlayerDataProperty.shoplvl, v => PlayerData.Player.shoplvl += v},
           { PlayerDataProperty.spacelvl, v => PlayerData.Player.spacelvl += v},
           { PlayerDataProperty.spaceupgradelvl, v => PlayerData.Player.SpaceUpgradelvl += v },
           { PlayerDataProperty.timesplayedspace, v => PlayerData.Player.TimesPlayedSpace += v},
           { PlayerDataProperty.userstats, v => PlayerData.Player.userstats += v},
           { PlayerDataProperty.week, v => PlayerData.Player.week += v},
        };

    // returns current property level after adjustment
    public static int AddToProperty(PlayerDataProperty prop, int add)
    {
        if (PlayerData.Player == null) return 0;
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
}
