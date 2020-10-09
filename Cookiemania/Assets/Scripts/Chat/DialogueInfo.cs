using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueInfo
{
    // item 1 is the characters unique name, item 2 is their line
    public List<Tuple<string, string>> dialogues;
    public EventManager.OnComplete runOnComplete;
    public Dictionary<string, Tuple<string, Sprite>> characterDictionary;
    // rewards given on completion of the ENTIRE dialogue
    // rewards not given mid dialogue
    public List<Tuple<ScriptConstants.RewardKeyword, int>> rewards;
    public string nextEvent;
    public string uniqueName;

    public DialogueInfo(
        string uniqueName,
        EventManager.OnComplete runOnComplete,
        Dictionary<string, Tuple<string, Sprite>> characterDictionary,
        string nextEvent = "",
        List<Tuple<string, string>> dialogues = null,
        List<Tuple<ScriptConstants.RewardKeyword, int>> rewards = null)
    {
        this.dialogues = dialogues != null ?
            dialogues : new List<Tuple<string, string>>();
        this.rewards = rewards != null ?
            rewards : new List<Tuple<ScriptConstants.RewardKeyword, int>>();
        this.runOnComplete = runOnComplete;
        this.characterDictionary = characterDictionary;
        this.nextEvent = nextEvent;
        this.uniqueName = uniqueName;
    }

    // if no unique name is given, argument will be provided as most recent talking
    // character
    public void AddDialogue(string dialogue, string charUniqueName = "")
    {
        if (charUniqueName == "")
        {
            charUniqueName = dialogues.Last().Item1;
        }
        dialogues.Add(new Tuple<string, string>(charUniqueName, dialogue));
    }
}