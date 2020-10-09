using System;
using System.Collections.Generic;
using UnityEngine;
using static ScriptConstants;

[Serializable]
public class ChoiceInfo
{
    // need to register selected choices to the choices made list on the global variable
    // tracker
    public string UniqueName;
    public Sprite CharacterImage;
    public string CharacterName;
    public string Prompt;
    // on each branch 1 etc declaration makes a new choice with empty string
    // and a new list of specific rewards in the rewards list
    // next line fills in text for declaration
    // reward lines fill in the reward list
    public List<string> Choices = new List<string>();
    public List<List<Tuple<RewardKeyword, int>>> Rewards =
        new List<List<Tuple<RewardKeyword, int>>>();
    public List<bool> ChoiceEarlyExits = new List<bool>();
    // key: index of choice in Choices, value: its dialogue's branch name
    public Dictionary<int, string> ChoiceDialogueDictionary =
        new Dictionary<int, string>();
    public EventManager.OnChoiceComplete RunOnComplete;

    public ChoiceInfo(string uniqueName)
    {
        CharacterName = "";
        Prompt = "";
        this.UniqueName = uniqueName;
    }

    public void AddChoice(string dialogueLine, bool earlyExit = false)
    {
        Choices.Add(dialogueLine);
        ChoiceEarlyExits.Add(earlyExit);
        Rewards.Add(new List<Tuple<RewardKeyword, int>>());
        ChoiceDialogueDictionary.Add(Choices.Count, "");
    }

    public bool IsFilledOut()
    {
        // may need to check for having an oncomplete and 
        // for having a next event?
        return CharacterImage != null && CharacterName != "" &&
            Prompt != "" && Choices.Count >= 1 &&
            Rewards.Count >= 1;
    }

    public ChoiceInfo(Sprite charImage, string charName, string choicePrompt,
        List<string> choices, List<List<Tuple<RewardKeyword, int>>> rewards,
        List<bool> choiceHasEarlyEnd,
        EventManager.OnChoiceComplete onComplete, int choiceLimit = 4)
    {
        if (choices.Count > choiceLimit)
        {
            Debug.LogError("too many choices");
            return;
        }
        if (choices.Count != rewards.Count)
        {
            Debug.LogError("need rewards for all choices, can be money 0 if " +
                "nothing is desired");
            return;
        }
        this.CharacterImage = charImage;
        this.CharacterName = charName;
        this.Prompt = choicePrompt;
        this.RunOnComplete = onComplete;
        this.Choices = new List<string>();
        this.Rewards = new List<List<Tuple<RewardKeyword, int>>>();
        this.ChoiceEarlyExits = new List<bool>();
        foreach (var choice in choices)
        {
            this.Choices.Add(choice);
        }
        foreach (var earlyEnd in choiceHasEarlyEnd)
        {
            this.ChoiceEarlyExits.Add(earlyEnd);
        }
        foreach (var reward in rewards)
        {
            this.Rewards.Add(reward);
        }
    }
}