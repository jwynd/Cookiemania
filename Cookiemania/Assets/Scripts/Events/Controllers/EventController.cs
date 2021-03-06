﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Parsing_Utilities;
using static Tracking.LocationUtils;

// carries through all the sequences for a single event
// should be triggered by an event manager
// uses the choice and dialogue prefab on the event manager
public class EventController : MonoBehaviour
{
    private EventInfo info;
    private Queue<EventInfo> eventQueue = new Queue<EventInfo>();
    // has some info that needs to be processed after the dialogue plays
    private DialogueInfo lastDialoguePlayed = null;

    public DialogueController.OnComplete OnDialogueComplete;
    public ChoiceController.OnComplete OnChoiceComplete;
    public EmailController.OnComplete OnEmailComplete;

    private bool runningDialogueEvent = false;
    private float timeScale = 1f;

    private Dictionary<Locale, Queue<EventInfo>> DelayedEvents =
        new Dictionary<Locale, Queue<EventInfo>>();
    private Locale currentLocation = Locale.WebsiteTab;

    public void DialogueComplete(string nextBranch)
    {
        NextBranch(nextBranch);
    }

    public void ChoiceComplete(string nextBranch, string choicePrompt,
        string choiceMade, List<Tuple<RewardKeyword, int>> rewards,
        TypeKeyword type)
    {
        EventManager.Instance.ChoiceMade(info.UniqueName, choicePrompt, choiceMade);
        EventManager.Instance.DistributeRewards(rewards);
        if (!type.IsEmail())
        {
            NextBranch(nextBranch);
        }
    }

    public void EmailComplete(EventInfo emailEvent, bool delayedCallback)
    {
        // need to check all the triggers 
        if (delayedCallback)
        {
            // delay
            IEnumerator coroutine = DelayedEmailComplete(emailEvent);
            StartCoroutine(coroutine);
        }
        else
        {
            // immediate
            EmailCompleteImmediate(emailEvent);
        }
    }

    private void EmailCompleteImmediate(EventInfo emailEvent)
    {
        TriggerDialogueEvents(emailEvent.GetEmailDialogues());
        EventComplete(emailEvent, false);
    }

    private IEnumerator DelayedEmailComplete(EventInfo emailEvent)
    {
        yield return new WaitForSecondsRealtime(1.0f);
        EmailCompleteImmediate(emailEvent);
    }

    private void TriggerDialogueEvents(List<DialogueInfo> lists)
    {
        foreach (var info in lists)
        {
            foreach (var triggerable in info.DirectlyTriggeredEvents)
            {
                EventManager.Instance.TriggerEvent(triggerable);
            }
        }
    }

    private void NextBranch(string nextBranch)
    {
        runningDialogueEvent = true;
        if (lastDialoguePlayed != null)
        {
            // run through the post dialogue wrap ups (e.g. early exits 
            // and event triggers)
            foreach (var eventName in lastDialoguePlayed.DirectlyTriggeredEvents)
            {
                EventManager.Instance.TriggerEvent(eventName);
            }
            // this must be last
            var shouldExit = lastDialoguePlayed.ExitsEvent;
            lastDialoguePlayed = null;
            if (shouldExit)
            {
                EventComplete(info, true);
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
            EventComplete(info, true);
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
                lastDialoguePlayed, OnDialogueComplete);
        }
        else
        {
            // is choice, activate the choice controller
            lastDialoguePlayed = null;
            var choiceInfo = info.GetChoice(branchTuple.Item2);
            EventManager.Instance.ChoicePrefab.
                GetComponent<ChoiceController>().Initialize(
                choiceInfo, info.EventType, OnChoiceComplete);
        }
    }

    public void Awake()
    {
        OnDialogueComplete = DialogueComplete;
        OnChoiceComplete = ChoiceComplete;
        OnEmailComplete = EmailComplete;
    }

    public void Start()
    {
        LoadedGame();
    }

    private void LoadedGame()
    {
        if (SaveSystem.DontLoad()) return;
        foreach (var ename in PlayerData.Player.QueuedEvents)
        {
            eventQueue.Enqueue(EventManager.Instance.GetEvent(ename));
        }
        foreach (var kvpair in PlayerData.Player.DelayedEvents)
        {
            var q = new Queue<EventInfo>();
            foreach (var ename in kvpair.Value)
            {
                q.Enqueue(EventManager.Instance.GetEvent(ename));
            }
            DelayedEvents.Add(kvpair.Key, q);
        }
    }

    public void ConnectPlayerLocationListener()
    {
        PlayerData.Player.Location.Updated.AddListener(LocationChanged);
    }

    private void LocationChanged(Locale previous, Locale current)
    {
        currentLocation = current;
        // need a reverse alias
        foreach (var toCheck in DelayedEvents.Keys)
        {
            if (toCheck.LocaleAlias().Contains(currentLocation))
            {
                var queue = DelayedEvents[toCheck];
                // player legit can't move while this is happening
                // but best to be double checking that their locale hasn't changed
                while (queue.Count > 0)
                {
                    eventQueue.Enqueue(queue.Dequeue()); 
                }
            }
        }
        foreach (var e in eventQueue)
        {
            Debug.LogWarning(e.UniqueName);
        }
        if (!runningDialogueEvent && eventQueue.Count > 0) RunEvent(eventQueue.Dequeue(), true);
        
    }

    // is triggered by something else, but event will run until completion
    // compare to a quest stage in an rpg
    public void RunEvent(EventInfo eventInfo, bool ignoreDelay = false)
    {
        // reference copy, we want to effect the original
        Debug.LogWarning(eventInfo.UniqueName);
        Debug.LogWarning(eventInfo.EventListening);
        // dont want to rerun events we've done before
        if (PlayerData.Player.CompletedEvents.Contains(eventInfo.UniqueName))
        {
            return;
        }
        // dialogue events need dialogue control to run
        if (runningDialogueEvent &&
            eventInfo.EventType == TypeKeyword.Dialogue ||
            eventInfo.EventType == TypeKeyword.Tutorial)
        {
            eventQueue.Enqueue(eventInfo);
            return;
        }
#if UNITY_EDITOR
        eventInfo.PrintInformation();
#endif 
        RunEventByType(eventInfo, ignoreDelay);
    }

    private void RunEventByType(EventInfo eventInfo, bool ignoreDelay)
    {
        eventInfo.EventListening = false;
        if (eventInfo.EventType.IsEmail())
        {
            AddEmail(eventInfo);
            return;
        }
        if (ignoreDelay)
            ImmediateRun(eventInfo);
        else
            ImmediateOrDelayedRun(eventInfo);
    }

    private void ImmediateOrDelayedRun(EventInfo eventInfo)
    {
        var currentAcceptableLocales = eventInfo.DelayOption.LocaleAlias();
        if (currentAcceptableLocales.Contains(currentLocation))
        {
            ImmediateRun(eventInfo);
            return;
        }
        if (DelayedEvents.TryGetValue(eventInfo.DelayOption, 
            out Queue<EventInfo> queue))
        {
            queue.Enqueue(eventInfo);
        }
        else
        {
            queue = new Queue<EventInfo>();
            queue.Enqueue(eventInfo);
            DelayedEvents.Add(eventInfo.DelayOption, queue);
        }
    }

    private void ImmediateRun(EventInfo eventInfo)
    {
        var possibleScale = PauseMenu.PauseWithoutScreen();
        timeScale = possibleScale > 0 ? possibleScale : timeScale;
        info = eventInfo;
        if (eventInfo.EventType == TypeKeyword.Reward)
        {
            EventComplete(eventInfo, false);
            return;
        }
        NextBranch(EventInfo.FIRST_BRANCH);
    }

    // all this function should do is add an email to the email controller
    // need to pass the function that returns what the email choice made
    // was when choices are made
    private void AddEmail(EventInfo info)
    {
        info.Email.choiceAction = OnChoiceComplete;
        info.Email.emailComplete = OnEmailComplete;
        if (EventManager.Instance.Email)
        {
            EventManager.Instance.Email.AddEmail(info);
        }
        else
        {
            Debug.LogError("Event manager instance needs a reference to the" +
                " email controller");
        }
    }

    public Queue<string> GetEventQueue()
    {
        var eq = new Queue<string>();
        foreach (var item in eventQueue) {
            eq.Enqueue(item.UniqueName);
        }
        return eq;
    }

    public Dictionary<Locale, Queue<string>> GetDelayedEvents()
    {
        var de = new Dictionary<Locale, Queue<string>>();
        foreach (var kv in DelayedEvents)
        {
            var list = new Queue<string>();
            foreach (var einfo in kv.Value)
            {
                list.Enqueue(einfo.UniqueName);
            }
            de.Add(kv.Key, list);
        }
        return de;
    }

    public bool CanSaveLoad()
    {
        return !runningDialogueEvent;
    }

    private void EventComplete(EventInfo eInfo, bool hadDialogue)
    {
        EventManager.Instance.EventComplete(
            eInfo.UniqueName, eInfo.EventCompleteReward);
        PlayerData.Player.CompletedEvents.Add(eInfo.UniqueName);
        if (hadDialogue)
            runningDialogueEvent = false;
        PauseMenu.ResumeWithoutScreen(timeScale);
        if (eventQueue.Count > 0)
        {
            RunEvent(eventQueue.Dequeue());
        }
    }
}
