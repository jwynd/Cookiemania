using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using static Parsing_Utilities;
using static Email_Utilities;

[Serializable]
public class EventInfo
{
    public const string LAST_BRANCH = "end";
    public const string FIRST_BRANCH = "first";
    public const string EMAIL_SUBJECT = "subject";
    public const string EMAIL_BODY = "body";

    // conditions are always the first time the amount has been reached for 
    // all trigger types
    public List<Tuple<TriggerKeyword, int>> TriggeringConditions =
       new List<Tuple<TriggerKeyword, int>>();

    // whether this event needs all triggering conditions
    // to be reached or just one of them to proc
    public bool AllTriggersNeeded = true;
    public string UniqueName { get; private set; }

    // set it to false when event runs first time
    public bool EventListening = true;

    // if an email, will fill this info at the end of the event creation
    public EmailInfo Email = null;

    // for event type email: need a subject line, a sender, an email body
    // and can send images using the background dictionary as attachments
    // can have up to one choice that comes after the email dialogue
    // that pops up when player tries to close the email

    // in the branching dictionary --> will have "subject", "attachments",
    // and only one dialogue (still first) and at most one choice (linked
    // after the only dialogue)
    public TypeKeyword EventType = TypeKeyword.Dialogue;
    public DelayedRunKeyword DelayOption = DelayedRunKeyword.None;

    // add to playerdata on event complete regardless of choices made
    // if there is no neutral reward associate your rewards with the choices obv
    public List<Tuple<RewardKeyword, int>> EventCompleteReward =
       new List<Tuple<RewardKeyword, int>>();

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
        string dialogueLine, string characterName,
        Sprite background = null)
    {
        if (characterName == null || characterName == "")
        {
            throw new Exception("character name must be defined for multi branch" +
                " dialogue writing");
        }
        var dialogues = GetAllDialogues(dialogueBranchNames);
        foreach (var dialogue in dialogues)
        {
            dialogue.AddDialogue(dialogueLine, characterName, background);
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

    public List<DialogueInfo> GetAllDialogues(List<string> dialogueBranchNames,
        bool noExceptions = false)
    {
        List<DialogueInfo> dialogues = new List<DialogueInfo>();
        foreach (var name in dialogueBranchNames)
        {
            if (BranchingDictionary.TryGetValue(name,
                out Tuple<bool, int> dictionaryTuple))
            {
                if (!dictionaryTuple.Item1)
                {
                    if (noExceptions)
                    {
                        continue;
                    }
                    throw new Exception("name is not a dialogue: " + name);
                }
                var dialogue = GetDialogue(dictionaryTuple.Item2);
                dialogues.Add(dialogue);
            }
            else
            {
                if (noExceptions)
                {
                    continue;
                }
                throw new Exception("name not found for " +
                    "multi-dialogue writing: " + name);
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
        // Choices can be empty
        if (Choices.Count < 1)
            return null;
        return Choices.Last();
    }

    public DialogueInfo GetLastDialogue()
    {
        // Dialogues must never be empty
        //if (Dialogues.Count < 1)
        //    return null;
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

    public void FinalizeCreation()
    {
        GetLastDialogue().NextBranch = LAST_BRANCH;
        GetLastDialogue().ExitsEvent = true;
        // no need to create EmailInfo, which is last step
        // for email events
        if (!EventType.IsEmail())
        {
            return;
        }
        string sender = "";
        string subject = "";
        string body = "";
        // obviously if this doesnt work theres a problem with 
        // the IsEmail(), like wth howd you even break that
        EmailCategory type = CATEGORY_FROM_TYPE[EventType];
        List<Tuple<string, Sprite>> attachments =
            new List<Tuple<string, Sprite>>();
        ChoiceInfo choice = GetLastChoice();
        ExtractSenderAndSubject(ref sender, ref subject);
        ExtractBodyAndAttachments(ref body, ref attachments);

        if (subject == "" ||
            body == "" ||
            sender == "")
        {
            throw new Exception("cannot create email without defining a subject: " +
                subject + ", a body: " + body + " and a sender: " + sender);
        }
        Email = new EmailInfo(subject, body, sender, type, choice, attachments);
    }

    private void ExtractSenderAndSubject(ref string sender, ref string subject)
    {
        if (BranchingDictionary.TryGetValue(
                    EMAIL_SUBJECT, out Tuple<bool, int> dInfo))
        {
            var senderSubject = GetDialogue(dInfo.Item2).Dialogues.First();
            sender = senderSubject.Item1;
            subject = senderSubject.Item2;
        }
    }

    private void ExtractBodyAndAttachments(ref string body, 
        ref List<Tuple<string, Sprite>> attachments)
    {
        if (BranchingDictionary.TryGetValue(
                    EMAIL_BODY, out Tuple<bool, int> bodyInfo))
        {
            var tuples = GetDialogue(bodyInfo.Item2).Dialogues;
            var attachmentSprites = GetDialogue(bodyInfo.Item2).Backgrounds;
            var tupleList = new List<string>();

            foreach (var tuple in tuples)
            {
                tupleList.Add(tuple.Item2);
            }

            foreach (var attachment in attachmentSprites)
            {
                if (attachment != null)
                    attachments.Add(new Tuple<string, Sprite>
                        ("attachment" + (attachments.Count + 1).ToString(), attachment));
            }
            body = string.Join("\n", tupleList);
        }
    }

    // will be using this to track parsing implementation progression
    public void PrintInformation()
    {
        Debug.Log("Event: " + UniqueName);
        Debug.Log("Event type: " + EventType);
        Debug.Log("Triggering Conditions: " + string.Join(", ", TriggeringConditions));
        Debug.Log("All triggers needed? " + AllTriggersNeeded);
        Debug.Log("Delayed until: " + DelayOption);
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
