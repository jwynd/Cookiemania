using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ScriptConstants;
using System.Linq;

// carries through all the sequences for a single event
// should be triggered by an event manager
// uses the choice and dialogue prefab on the event manager
public class EventController : MonoBehaviour
{
    private EventInfo info;
    //private bool inChoice = false;

    public DialogueController.OnComplete onDialogueComplete;

    public ChoiceController.OnComplete onChoiceComplete;

    public void DialogueComplete(string nextBranch)
    {
        NextBranch(nextBranch);
    }

    public void ChoiceComplete(string nextBranch, int choiceNumber, 
        List<Tuple<RewardKeyword, int>> rewards)
    {
        EventManager.Instance.ChoiceMade(info.UniqueName, choiceNumber);
        EventManager.Instance.DistributeRewards(rewards);
        NextBranch(nextBranch);
    }

    private void NextBranch(string nextBranch)
    {
        Debug.Log("running branch " + nextBranch);
        if (nextBranch == EventInfo.LAST_BRANCH)
        {
            EventComplete();
            return;
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
            var branch = info.GetDialogue(branchTuple.Item2);
            // char dictionary is handled on event manager awake
            EventManager.Instance.DialoguePrefab.
                GetComponent<DialogueController>().Initialize(branch.Dialogues,
                onDialogueComplete, branch.NextBranch, branch.Backgrounds);
        }
        else
        {
            // is choice, activate the choice controller
            var branch = info.GetChoice(branchTuple.Item2);
            EventManager.Instance.ChoicePrefab.
                GetComponent<ChoiceController>().Initialize(branch.CharacterName,
                branch.CharacterImage, branch.Prompt, branch.Choices,
                branch.Rewards, branch.ChoiceDialogueDictionary.Values.ToList(), onChoiceComplete,
                branch.Background);
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
        info = eventInfo;
        info.EventListening = false;
        if (!info.RequiresDialogueControl)
        {
            EventComplete();
            return;
        }
        NextBranch(EventInfo.FIRST_BRANCH);
    }

    private void EventComplete()
    {
        EventManager.Instance.EventComplete(
            info.UniqueName, info.EventCompleteReward);
    }
}
