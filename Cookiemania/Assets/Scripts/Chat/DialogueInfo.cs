using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static ScriptConstants;

[Serializable]
public class DialogueInfo
{
    // item 1 is the characters unique name, item 2 is their line
    public List<Tuple<string, string>> Dialogues;
    public EventManager.OnComplete RunOnComplete;
    public Dictionary<string, Tuple<string, Sprite>> CharacterDictionary;
    // rewards given on completion of the ENTIRE dialogue
    // rewards not given mid dialogue
    public List<Tuple<RewardKeyword, int>> Rewards;
    public string NextBranch;
    public string UniqueName;

    public DialogueInfo(
        string uniqueName,
        EventManager.OnComplete runOnComplete,
        Dictionary<string, Tuple<string, Sprite>> characterDictionary,
        string nextEvent = "",
        List<Tuple<string, string>> dialogues = null,
        List<Tuple<RewardKeyword, int>> rewards = null)
    {
        this.Dialogues = dialogues != null ?
            dialogues : new List<Tuple<string, string>>();
        this.Rewards = rewards != null ?
            rewards : new List<Tuple<RewardKeyword, int>>();
        this.RunOnComplete = runOnComplete;
        this.CharacterDictionary = characterDictionary;
        this.NextBranch = nextEvent;
        this.UniqueName = uniqueName;
    }

    // if no unique name is given, argument will be provided as most recent talking
    // character
    public void AddDialogue(string dialogue, string charUniqueName = "")
    {
        if (charUniqueName == "")
        {
            charUniqueName = Dialogues.Last().Item1;
        }
        Dialogues.Add(new Tuple<string, string>(charUniqueName, dialogue));
    }
}