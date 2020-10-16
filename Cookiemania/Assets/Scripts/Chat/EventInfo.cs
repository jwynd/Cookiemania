using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EventInfo
{
    public const string LAST_BRANCH = "end";
    public const string FIRST_BRANCH = "first";

    public Tuple<ScriptConstants.TriggerKeyword, int> TriggeringAction;
    public string UniqueName { get; private set; }
    // set it to false when event runs first time
    public bool EventListening = true;
    // these events should all need the dialogue box stuff, but if they dont
    // when the event runs, they'll just trigger the eventcompletereward and exit
    public bool RequiresDialogueControl = true;

    // add to playerdata on event complete regardless of choices made
    // if there is no neutral reward associate your rewards with the choices obv
    public List<Tuple<ScriptConstants.RewardKeyword, int>> EventCompleteReward;

    public int BranchID
    {
        get; private set;
    }
    // the tuple is --> true means its just a dialogue, false is a choice
    // and the int is its index in the list
    // NOTE: the OnComplete for each choices / dialogues should be 
    // starting an event with a string in the branchingDictionary
    // 
    public Dictionary<string, Tuple<bool, int>> BranchingDictionary =
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

    public ChoiceInfo GetChoice(int index)
    {
        if (index >= Choices.Count)
        {
            throw new IndexOutOfRangeException();
        }
        return Choices[index];
    }

    public DialogueInfo GetDialogue(int index)
    {
        if (index >= Dialogues.Count)
        {
            throw new IndexOutOfRangeException();
        }
        return Dialogues[index];
    }

    // will be using this to track parsing implementation progression
    public void PrintInformation()
    {
        Debug.Log(string.Join(" ", BranchingDictionary));
        foreach (var dialogue in Dialogues)
        {
            Debug.Log(dialogue.UniqueName);
            Debug.Log(string.Join(" ", dialogue.Dialogues));
        }
        foreach (var choice in Choices)
        {
            Debug.Log(choice.UniqueName);
            Debug.Log(choice.Prompt);
            Debug.Log(string.Join(" ", choice.Choices));
        }
    }
}
