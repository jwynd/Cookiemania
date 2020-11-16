using System;
using System.Collections.Generic;
using UnityEngine;
using static Parsing_Utilities;

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
    private float timeScale = 1f;
    private EmailController emailController;

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
        runningDialogueEvent = true;
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
            EventComplete();
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
                GetComponent<DialogueController>().Initialize(
                lastDialoguePlayed, onDialogueComplete);
        }
        else
        {
            // is choice, activate the choice controller
            lastDialoguePlayed = null;
            var choiceInfo = info.GetChoice(branchTuple.Item2);
            EventManager.Instance.ChoicePrefab.
                GetComponent<ChoiceController>().Initialize(
                choiceInfo, onChoiceComplete);
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
        // dialogue events need dialogue control to run
        if (runningDialogueEvent && 
            eventInfo.EventType == TypeKeyword.Dialogue)
        {
            eventQueue.Enqueue(eventInfo);
            return;
        }
#if UNITY_EDITOR
        eventInfo.PrintInformation();
#endif
        var possibleScale = PauseMenu.PauseWithoutScreen();
        timeScale = possibleScale > 0 ? possibleScale : timeScale;
        RunEventByType(eventInfo);
    }

    private void RunEventByType(EventInfo eventInfo)
    {
        info = eventInfo;
        info.EventListening = false;
        if (eventInfo.EventType.IsEmail())
        {
            AddEmail(info);
            return;
        }
        switch (eventInfo.EventType)
        {
            case TypeKeyword.Dialogue:
                NextBranch(EventInfo.FIRST_BRANCH);
                break;
            case TypeKeyword.Reward:
                EventComplete();
                break;
            default:
                Debug.LogError("unimplemented event type for event " +
                    "controller");
                break;
        }
    }

    // all this function should do is add an email to the email controller
    // need to pass the function that returns what the email choice made
    // was when choices are made
    private void AddEmail(EventInfo info)
    {
        if (EventManager.Instance.Email)
        {
            EventManager.Instance.Email.AddEmail(info, this);
        }
        else
        {
            Debug.LogError("Event manager instance needs a reference to the" +
                " email controller");
        }
    }

    private void EventComplete()
    {
        EventManager.Instance.EventComplete(
            info.UniqueName, info.EventCompleteReward);
        runningDialogueEvent = false;
        PauseMenu.ResumeWithoutScreen(timeScale);
        if (eventQueue.Count > 0)
        {
            RunEvent(eventQueue.Dequeue());
        }
    }
}
