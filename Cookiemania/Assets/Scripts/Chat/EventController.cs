using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static ScriptConstants;

// carries through all the sequences for a single event
// should be triggered by an event manager
// uses the choice and dialogue prefab on the event manager
public class EventController : MonoBehaviour
{
    private EventInfo info;
    private Queue<EventInfo> eventQueue = new Queue<EventInfo>();
    // has some info that needs to be processed after the dialogue plays
    private DialogueInfo lastDialoguePlayed = null;

    public DialogueController.OnComplete onDialogueComplete;

    public ChoiceController.OnComplete onChoiceComplete;
    private bool runningDialogueEvent = false;

    public void DialogueComplete(string nextBranch)
    {
        NextBranch(nextBranch);
    }

    public void ChoiceComplete(string nextBranch, string choicePrompt, 
        string choiceMade, List<Tuple<RewardKeyword, int>> rewards)
    {
        EventManager.Instance.ChoiceMade(info.UniqueName, choicePrompt, choiceMade);
        EventManager.Instance.DistributeRewards(rewards);
        NextBranch(nextBranch);
    }

    private void NextBranch(string nextBranch)
    {
        if (lastDialoguePlayed != null)
        {
            // run through the post dialogue wrap ups (e.g. early exits 
            // and event triggers)
            foreach(var eventName in lastDialoguePlayed.DirectlyTriggeredEvents)
            {
                EventManager.Instance.TriggerEvent(eventName);
            }
            // this must be last
            var shouldExit = lastDialoguePlayed.ExitsEvent;
            lastDialoguePlayed = null;
            if (shouldExit)
            {
                EventComplete();
                return;
            }
        }
        if (info.BranchingDictionary.TryGetValue(nextBranch,
            out Tuple<bool, int> branchTuple))
        {
            RunDialogueOrChoice(branchTuple);
        }
        else
        {
            Debug.LogError("branch: " + nextBranch + 
                " not found in triggered event " + info.UniqueName);
        }
    }

    private void RunDialogueOrChoice(Tuple<bool, int> branchTuple)
    {
        if (branchTuple.Item1)
        {
            // is dialogue, activate the dialogue controller
            lastDialoguePlayed = info.GetDialogue(branchTuple.Item2);
            // char dictionary is handled on event manager awake
            EventManager.Instance.DialoguePrefab.
                GetComponent<DialogueController>().Initialize(lastDialoguePlayed.Dialogues,
                onDialogueComplete, lastDialoguePlayed.NextBranch, lastDialoguePlayed.Backgrounds);
        }
        else
        {
            // is choice, activate the choice controller
            lastDialoguePlayed = null;
            var choiceInfo = info.GetChoice(branchTuple.Item2);
            EventManager.Instance.ChoicePrefab.
                GetComponent<ChoiceController>().Initialize(choiceInfo.CharacterName,
                choiceInfo.CharacterImage, choiceInfo.Prompt, choiceInfo.Choices,
                choiceInfo.Rewards, choiceInfo.ChoiceDialogueDictionary.Values.ToList(), onChoiceComplete,
                choiceInfo.Background);
        }
    }

    public void Awake()
    {
        onDialogueComplete = DialogueComplete;
        onChoiceComplete = ChoiceComplete;
    }

    // is triggered by something else, but event will run until completion
    // compare to a quest stage in an rpg
    public void RunEvent(EventInfo eventInfo)
    {
        // reference copy, we want to effect the original
        if (!eventInfo.EventListening)
            return;
        if (runningDialogueEvent)
        {
            eventQueue.Enqueue(eventInfo);
            return;
        }
#if UNITY_EDITOR
        eventInfo.PrintInformation();
#endif
        info = eventInfo;
        info.EventListening = false;
        if (!info.RequiresDialogueControl)
        {
            EventComplete();
            return;
        }
        runningDialogueEvent = true;
        NextBranch(EventInfo.FIRST_BRANCH);
    }

    private void EventComplete()
    {
        EventManager.Instance.EventComplete(
            info.UniqueName, info.EventCompleteReward);
        runningDialogueEvent = false;
        if (eventQueue.Count > 0)
        {
            RunEvent(eventQueue.Dequeue());
        }
    }
}
