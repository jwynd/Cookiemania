using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChoiceInfo
{
    // need to register selected choices to the choices made list on the global variable
    // tracker
    public string uniqueName;
    public Sprite charImage;
    public string charName;
    public string choicePrompt;
    // on each branch 1 etc declaration makes a new choice with empty string
    // and a new list of specific rewards in the rewards list
    // next line fills in text for declaration
    // reward lines fill in the reward list
    public List<string> choices = new List<string>();
    public List<List<Tuple<ScriptConstants.RewardKeyword, int>>> rewards =
        new List<List<Tuple<ScriptConstants.RewardKeyword, int>>>();
    public List<bool> choiceHasEarlyEnd = new List<bool>();
    public Dictionary<int, string> choiceNumberToNextEvent =
        new Dictionary<int, string>();
    public EventManager.OnChoiceComplete onComplete;

    public ChoiceInfo(string uniqueName)
    {
        charName = "";
        choicePrompt = "";
        this.uniqueName = uniqueName;
    }

    public void AddChoice(string dialogueLine, bool earlyExit = false)
    {
        choices.Add(dialogueLine);
        choiceHasEarlyEnd.Add(earlyExit);
        rewards.Add(new List<Tuple<ScriptConstants.RewardKeyword, int>>());
        choiceNumberToNextEvent.Add(choices.Count, "");
    }

    public bool IsFilledOut()
    {
        // may need to check for having an oncomplete and 
        // for having a next event?
        return charImage != null && charName != "" &&
            choicePrompt != "" && choices.Count >= 1 &&
            rewards.Count >= 1;
    }

    public ChoiceInfo(Sprite charImage, string charName, string choicePrompt,
        List<string> choices, List<List<Tuple<ScriptConstants.RewardKeyword, int>>> rewards,
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
        this.charImage = charImage;
        this.charName = charName;
        this.choicePrompt = choicePrompt;
        this.onComplete = onComplete;
        this.choices = new List<string>();
        this.rewards = new List<List<Tuple<ScriptConstants.RewardKeyword, int>>>();
        this.choiceHasEarlyEnd = new List<bool>();
        foreach (var choice in choices)
        {
            this.choices.Add(choice);
        }
        foreach (var earlyEnd in choiceHasEarlyEnd)
        {
            this.choiceHasEarlyEnd.Add(earlyEnd);
        }
        foreach (var reward in rewards)
        {
            this.rewards.Add(reward);
        }
    }
}