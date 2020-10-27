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
    public List<Tuple<ScriptConstants.RewardKeyword, int>> EventCompleteReward = 
       new List<Tuple<ScriptConstants.RewardKeyword, int>>();

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

    public EventInfo(string uniqueName, 
        bool hasDialogue = true)
    {
        UniqueName = uniqueName;
        BranchID = 0;
        if (hasDialogue)
        {
            var dInfo = new DialogueInfo(BranchID.ToString());
            // adding a default dialogue to event info for first dialogue
            AddDialogue(dInfo);
            BranchingDictionary.Add(FIRST_BRANCH,
                new Tuple<bool, int>(true, 0));
        }
    }

    public void SetNextBranch(string precedingBranch, string nextBranch)
    {
        if (BranchingDictionary.TryGetValue(precedingBranch, 
            out Tuple<bool, int> parent))
        {
            if (parent.Item1)
            {
                var dialogue = Dialogues[parent.Item2];
                dialogue.NextBranch = nextBranch;
            }
        }
        else
        {
            Debug.Log(precedingBranch + " " + nextBranch);
            throw new Exception("branch: " + precedingBranch + 
                " not found in branching dictionary");
        }
    }

    // write to multiple dialogues (for choice branches that should have the same dialogue
    // result
    public void MultiDialogueWrite(List<string> dialogueBranchNames, 
        string dialogueLine, string characterName)
    {
        var dialogues = GetAllDialogues(dialogueBranchNames);
        foreach (var dialogue in dialogues)
        {
            dialogue.AddDialogue(dialogueLine, characterName);
        }
    }

    public void MultiEventTriggerWrite(List<string> dialogueBranchNames, string eventName)
    {
        var dialogues = GetAllDialogues(dialogueBranchNames);
        foreach (var dialogue in dialogues)
        {
            dialogue.DirectlyTriggeredEvents.Add(eventName);
        }
    }

    public void MultiEarlyExitWrite(List<string> dialogueBranchNames)
    {
        var dialogues = GetAllDialogues(dialogueBranchNames);
        foreach (var dialogue in dialogues)
        {
            dialogue.ExitsEvent = true;
        }
    }

    private List<DialogueInfo> GetAllDialogues(List<string> dialogueBranchNames)
    {
        List<DialogueInfo> dialogues = new List<DialogueInfo>();
        foreach (var name in dialogueBranchNames)
        {
            if (BranchingDictionary.TryGetValue(name, out Tuple<bool, int> dictionaryTuple))
            {
                if (!dictionaryTuple.Item1)
                {
                    throw new Exception("name is not a dialogue: " + name);
                }
                var dialogue = GetDialogue(dictionaryTuple.Item2);
                dialogues.Add(dialogue);
            }
            else
            {
                throw new Exception("name not found for multi-dialogue writing: " + name);
            }
        }
        return dialogues;
    }

    public void AddDialogue(DialogueInfo dInfo)
    {
        //SetNextBranch(nextBranch, precedingBranch);
        Dialogues.Add(dInfo);
        BranchID++;
        BranchingDictionary.Add(dInfo.UniqueName,
            new Tuple<bool, int>(true, Dialogues.Count - 1));
    }

    public void AddChoice(ChoiceInfo cInfo)
    {
        Choices.Add(cInfo);
        BranchID++;
        BranchingDictionary.Add(cInfo.UniqueName,
            new Tuple<bool, int>(false, Choices.Count - 1));
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
        if (index >= Choices.Count || index < 0)
        {
            Debug.LogError("index not in range: " + 
                index + ", Range: " + Choices.Count);
        }
        return Choices[index];
    }

    public DialogueInfo GetDialogue(int index)
    {
        if (index >= Dialogues.Count || index < 0)
        {
            Debug.LogError("index not in range: " + 
                index + ", Range: " + Dialogues.Count);
        }
        return Dialogues[index];
    }

    // will be using this to track parsing implementation progression
    public void PrintInformation()
    {
        Debug.Log("Event: " + UniqueName);
        Debug.Log("Event completion rewards: " + string.Join(", ", EventCompleteReward));
        Debug.Log(string.Join(" ", BranchingDictionary));
        foreach (var dialogue in Dialogues)
        {
            Debug.Log(dialogue.UniqueName);
            Debug.Log(string.Join(" ", dialogue.Dialogues));
        }
        foreach (var choice in Choices)
        {
            choice.PrintInformation();
        }
    }
}
