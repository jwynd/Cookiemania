using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EventInfo
{
    public const string LAST_BRANCH = "end";
    public const string FIRST_BRANCH = "first";

    public Tuple<ScriptConstants.TriggerKeyword, int> triggeringAction;
    public string UniqueName { get; private set; }
    // set it to false when event runs and direct trigger was not used
    public bool eventListening = true;

    public int BranchID
    {
        get; private set;
    }
    // the tuple is --> true means its just a dialogue, false is a choice
    // and the int is its index in the list
    // NOTE: the OnComplete for each choices / dialogues should be 
    // starting an event with a string in the branchingDictionary
    // 
    public Dictionary<string, Tuple<bool, int>> branchingDictionary =
        new Dictionary<string, Tuple<bool, int>>();

    private List<DialogueInfo> Dialogues = new List<DialogueInfo>();
    private List<ChoiceInfo> Choices = new List<ChoiceInfo>();

    public EventInfo(string uniqueName)
    {
        UniqueName = uniqueName;
    }

    public void AddDialogue(DialogueInfo dInfo)
    {
        Dialogues.Add(dInfo);
        BranchID++;
    }

    public void AddChoice(ChoiceInfo cInfo)
    {
        Choices.Add(cInfo);
        BranchID++;
    }

    public ChoiceInfo GetLastChoice()
    {
        return Choices.Last();
    }

    public DialogueInfo GetLastDialogue()
    {
        return Dialogues.Last();
    }

    public void PrintInformation()
    {
        Debug.Log(string.Join(" ", branchingDictionary.Keys));
        Debug.Log(string.Join(" ", branchingDictionary.Values));
        foreach (var dialogue in Dialogues)
        {
            Debug.Log(dialogue.uniqueName);
            Debug.Log(string.Join(" ", dialogue.dialogues));
        }
        foreach (var choice in Choices)
        {
            Debug.Log(choice.uniqueName);
            Debug.Log(choice.choicePrompt);
            Debug.Log(string.Join(" ", choice.choices));
        }
    }
}
