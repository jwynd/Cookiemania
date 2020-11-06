using System;
using System.Collections.Generic;
using UnityEngine;
using static Parsing_Utilities;

[Serializable]
public class ChoiceInfo
{
    // need to register selected choices to the choices made list on the global variable
    // tracker
    public string UniqueName { get; private set; }
    public int ChoiceLimit { get; private set; }

    // these variables are only able to be set once, since they're set from a script

    private Sprite _charImage = null;
    public Sprite CharacterImage
    {
        get { return _charImage; }
        set { if (_charImage == null) _charImage = value; }
    }

    private string _charName = "";
    public string CharacterName 
    {
        get { return _charName; }
        set { if (_charName == "") _charName = value; }
    }

    private string _prompt = "";
    public string Prompt
    {
        get { return _prompt; }
        set { if (_prompt == "") _prompt = value; }
    }
    // on each branch 1 etc declaration makes a new choice with empty string
    // and a new list of specific rewards in the rewards list
    // next line fills in text for declaration
    // reward lines fill in the reward list
    public List<string> Choices = new List<string>();
    public List<List<Tuple<RewardKeyword, int>>> Rewards =
        new List<List<Tuple<RewardKeyword, int>>>();
    public Sprite Background = null;
    // key: index of choice in Choices, value: its dialogue's branch name
    public Dictionary<int, string> ChoiceDialogueDictionary =
        new Dictionary<int, string>();
    public Dictionary<string, int> ChoiceReverseDictionary =
        new Dictionary<string, int>();
    public ChoiceController.OnComplete RunOnComplete;

    public ChoiceInfo(string uniqueName, int choiceLimit = 4)
    {
        this.UniqueName = uniqueName;
        this.ChoiceLimit = choiceLimit;
    }

    public void AddChoice(string dialogueLine)
    {
        Choices.Add(dialogueLine);
        Rewards.Add(new List<Tuple<RewardKeyword, int>>());
        // shouldnt add here as we dont know the name of the choice yet
        //ChoiceDialogueDictionary.Add(Choices.Count, "");
    }

    public void AddReward(RewardKeyword keyword, int amount, int specificIndex = -1)
    {
        var index = specificIndex < 0 ? Rewards.Count - 1 : specificIndex;
        if (index >= Rewards.Count || index < 0)
        {
            throw new Exception(
                "cannot add reward at :" + index + " in choice: " + UniqueName);
        }
        Rewards[index].Add(
            new Tuple<RewardKeyword, int>(keyword, amount));
    }

    public void AddChoiceDialogueName(int index, string name)
    {
        //if (ChoiceDialogueDictionary.ContainsKey(index) ||
        //    ChoiceReverseDictionary.ContainsKey(name))
        //    throw new Exception("both name and index must be unique and only set once" +
        //        " for choice dialogue dictionaries :" + index.ToString() + " " + name);
        ChoiceDialogueDictionary.Add(index, name);
        ChoiceReverseDictionary.Add(name, index);
    }

    public bool IsFilledOutAndCorrect()
    {
        // may need to check for having an oncomplete and 
        // for having a next event?
        return CharacterImage != null && CharacterName != "" &&
            Prompt != "" && Choices.Count >= 1 &&
            Rewards.Count >= 1 && Choices.Count <= ChoiceLimit;
    }

    public void PrintInformation()
    {
        Debug.Log("Choice Name: " + UniqueName);
        Debug.Log("Prompt: " + Prompt);
        Debug.Log("Character Name: " + CharacterName);
        Debug.Log("Choices:\n" + string.Join(", ", Choices));
        Debug.Log("Rewards:\n");
        foreach (var reward in Rewards)
            Debug.Log(string.Join(", ", reward));
        Debug.Log("Choice Branch Name Dictionary:\n" + string.Join(", ", ChoiceDialogueDictionary));
    }

    public ChoiceInfo(Sprite charImage, string charName, string choicePrompt,
        List<string> choices, List<List<Tuple<RewardKeyword, int>>> rewards,
        ChoiceController.OnComplete onComplete, int choiceLimit = 4)
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
        foreach (var choice in choices)
        {
            this.Choices.Add(choice);
        }
        foreach (var reward in rewards)
        {
            this.Rewards.Add(reward);
        }
    }
}