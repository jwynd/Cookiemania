﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

using static PlayerData;
using static PlayerDataStatics;

public static class SaveSystem
{
    public static string SAVE_EXTENSION = ".SAV";

    // requires static fn on PlayerData: CreateSaveData()
    // should create based on the static player instance
    public static int Save(string filename)
    {
        if (Player == null) return 1;
        // can't save the game while running a dialogue event
        if (!EventManager.Instance.EventController.CanSaveLoad()) return 2;
        string path = Application.persistentDataPath + filename + SAVE_EXTENSION;
        FileStream stream = new FileStream(path, FileMode.Create);
        new BinaryFormatter().Serialize(stream, CreateSaveData());
        stream.Close();
        return 0;
    }

    public static int Load(string filename)
    {
        if (Player == null) return 1;
        string path = Application.persistentDataPath + filename + SAVE_EXTENSION;
        if (!File.Exists(path)) return 2;
        FileStream stream = new FileStream(path, FileMode.Open);
        LoadFromSave(new BinaryFormatter().Deserialize(stream) as SaveData);
        stream.Close();
        return 0;
    }

    public class SaveData
    {
        // turns on / off ability to purchase upgrade, if purchased does not
        // affect player stats (those stats must already be on the player data
        // in respective location
        public Dictionary<int, bool> UpgradesPurchased;
        public Queue<string> QueuedEvents;
        public HashSet<string> CompletedEvents;
        public Dictionary<Parsing_Utilities.Locale, Queue<string>> DelayedEvents;
        public Dictionary<string, List<Tuple<string, string>>> ChoicesMade;
        // all sent emails, bool = true if read
        public Queue<Tuple<string, bool, bool>> Inbox;
        // from money to coin jumps to what week it is
        // everything that is just a property -> integer
        // on the player class
        public Dictionary<PlayerDataProperty, int> PlayerLevels;
    }

    // creates the save data from the Player instance variable
    // uses references to originals as it gets saved to file immediately
    private static SaveData CreateSaveData()
    {
        // get current event queue from event manager
        // get current inbox from email manager?
        // get all player levels, can iterate properties through reflection or manually
        // set
        // get upgrades purchased, should be local dictionary already
        return new SaveData
        {
            ChoicesMade = Player.EventChoicesMade,
            CompletedEvents = Player.CompletedEvents,
            PlayerLevels = CreatePDPDictionary(),
            DelayedEvents = Player.DelayedEvents,
            Inbox = Player.Inbox,
            // Inbox = // something
            // UpgradesPurchased = // something
            QueuedEvents = Player.QueuedEvents,

        };
    }

    private static void LoadFromSave(SaveData data)
    {
        // need to tell event system to load
        // need to tell buttons to load
        Player.QueuedEvents = data.QueuedEvents;
        Player.CompletedEvents = data.CompletedEvents;
        Player.EventChoicesMade = data.ChoicesMade;
        Player.UpgradesPurchased = data.UpgradesPurchased;
        // Player.Inbox = data.Inbox;

        // last step is setting the player data integer properties
        SetPDP(data.PlayerLevels);
        // and the consistency checks for upgrades, events to ensure save data
        // wasn't corrupted
        // in short = will ensure minimum player data levels were met for variables
        // that only increment by making a deep copy of the player levels dictionary
        // and decrementing my way through it by the event & upgrade button's rewards
        // ensuring it doesn't go below zero
        // morality will also be checked (as it is event choice affected only), money 
        // will not since there is no lifetime money earned variable that would be able 
        // to ensure the player was actually able to purchase all the upgrades they were
        // assigned by the player data
    }

    private static Dictionary<PlayerDataProperty, int> CreatePDPDictionary()
    {
        var dict = new Dictionary<PlayerDataProperty, int>();
        // iterating through all the possible enum values
        foreach (PlayerDataProperty prop in Enum.GetValues(typeof(PlayerDataProperty)))
        {
            // adding zero to property just to find out the current value
            // coulda made ... another dictionary that doesn't do meaningless addition
            // but it would be more maintenance
            dict.Add(prop, AddToProperty(prop, 0));
        }
        return dict;
    }

    // sets normal player data properties from a dictionary that would be contained in the save data
    private static void SetPDP(Dictionary<PlayerDataProperty, int> dict)
    {
        // gets current amount, and adds required amount to set it to the value
        // part of the pair
        foreach (KeyValuePair<PlayerDataProperty, int> propPair in dict)
        {
            // e.g. player data has 12 rn, save data says it should be 3
            // send 3 - 12 to correct it
            // slightly extra function work, but only need to maintain 
            // one dictionary by hand
            AddToProperty(propPair.Key, 
                propPair.Value - AddToProperty(propPair.Key, 0));
        }
    }
}
