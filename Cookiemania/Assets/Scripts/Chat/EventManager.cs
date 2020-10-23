using General_Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static ScriptConstants;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    private int choiceLimit = 4;
    [SerializeField]
    private GameObject dialoguePrefab = null;
    [SerializeField]
    private GameObject choicePrefab = null;
    [SerializeField]
    private EventController eventController = null;
    [SerializeField]
    [Tooltip("The text files that contain events to be registered")]
    private List<TextAsset> eventTextFiles = new List<TextAsset>();
    [SerializeField]
    private List<CharacterInfo> characterList = new List<CharacterInfo>();
    [SerializeField]
    private bool useTestMode = true;
    private Dictionary<string, Tuple<string, Sprite>> characterDictionary =
        new Dictionary<string, Tuple<string, Sprite>>();
    private Dictionary<string, CharacterInfo> charInfoDictionary = 
        new Dictionary<string, CharacterInfo>();
    private Dictionary<string, EventInfo> eventDictionary = 
        new Dictionary<string, EventInfo>();
    //private Dictionary<string, EventController> activeEvents = new Dictionary<string, EventController>();

    public static EventManager Instance { get; private set; }

    public GameObject DialoguePrefab { get; private set; }

    public GameObject ChoicePrefab { get; private set; }

    public void EventComplete(string eventName, List<Tuple<RewardKeyword, int>> rewards)
    {
        // if we have events queued up from triggers will want to 
        // start them after distributing the rewards
        DistributeRewards(rewards);
    }

    public void ChoiceMade(string eventName, int choiceNumber)
    {
        if (eventDictionary.TryGetValue(eventName, out EventInfo value)) 
            Debug.LogWarning("need to track choices made on per event " +
                "basis --> should be list of tuples, this one is ( " + eventName + ", " +
                choiceNumber.ToString() + " )");
    }

    public void DistributeRewards(List<Tuple<RewardKeyword, int>> rewards)
    {
        Debug.LogWarning("need to add rewards for event completion");
    }

    private void ReadInFiles(List<TextAsset> textAssets)
    {
        char[] toTrim = { '\t' };
        // overall logic: event designation MUST come first
        // for dialogue --> add dInfo if dInfo.dialogues is not empty to the eventinfo
        // when a choice comes up OR when the event ends OR when a branch ends e.g. choice, 
        // branch_end, event_end, event_early_end
        // for choices --> need to go up to choice_end OR branch_start
        // OR event_end OR event_early_end
        // after all ACTUAL branches of a choice have been read through to their branch_end 
        // (branches can share dialogue / a branch_end btw) then all those branches will 
        // point to next dialogue as its next branch
        // last branch will point to "end" first branch will point to "start"

        // will need a moderately complex function for grabbing a branch and setting up 
        // either the choice or dialogue controller and activating it

        // triggering action happening should start that function at "start" for the 
        // given event
        foreach (var asset in textAssets)
        {
            CharacterInfo lastCharacterDesignated = null;
            bool insideChoice = false;
            bool insideChoiceBranchDialogue = false;
            List<string> choiceDialogueIndicesToWriteTo = new List<string>();
            EventInfo eInfo = null;
            var texts = asset.text.Split('\n');

            // will swap back to foreach loop if i dont end up needing the index
            for (int i = 0; i < texts.Length; i++)
            {
                var text = texts[i];
                var trimmedText = text.Trim(toTrim).Split(' ').ToList();
                if (ReadInContinues(trimmedText))
                {
                    continue;
                }

                // this first one needs to be lowercased and trimmed
                trimmedText[0] = trimmedText.First().ToLowerInvariant().Trim();
                if (trimmedText.First()[0] == DIALOGUE)
                {
                    var textLine = ExtractDialogue(trimmedText);
                    // only unset this when we hit the choice_end or branch_start keywords
                    if (insideChoice && !insideChoiceBranchDialogue)
                    {
                        // need to go down the prio list --> if prompt filled go to last 
                        // choice and replace the string there
                        // if that string is not "" throw an error cuz choices only get 
                        // one line of text.
                        //throw new NotImplementedException();
                        var choice = eInfo.GetLastChoice();
                        if (choice.Prompt == "")
                            choice.Prompt = textLine;
                        else
                            choice.Choices[choice.Choices.Count - 1] = textLine;
                    }
                    else if (insideChoice && insideChoiceBranchDialogue)
                    {
                        // need to make sure we fill the correct dialogue, not just 
                        // the most recent one
                        eInfo.MultiDialogueWrite(choiceDialogueIndicesToWriteTo, textLine, 
                            lastCharacterDesignated.UniqueName);
                    }
                    else
                    {
                        AddToDialogue(trimmedText, lastCharacterDesignated,
                            eInfo.GetLastDialogue());
                    }
                    continue;
                }
                ParsePossibleKeyword(ref lastCharacterDesignated, ref insideChoice, 
                    ref insideChoiceBranchDialogue, ref eInfo, trimmedText,
                    choiceDialogueIndicesToWriteTo);
            }
        }

    }

    private void ParsePossibleKeyword(ref CharacterInfo lastCharacterDesignated, 
        ref bool insideChoice, ref bool insideBranchDeclaration, 
        ref EventInfo eInfo, List<string> trimmedText, 
        List<string> choiceDialogueIndicesToWriteTo)
    {
        var keywordMatch = trimmedText.First();
        if (BASE_KEYWORDS.TryGetValue(
            keywordMatch, out BaseKeyword keyword))
        {
            ResolveKeyword(keyword, trimmedText, ref eInfo,
                ref lastCharacterDesignated, ref insideChoice,
                ref insideBranchDeclaration, choiceDialogueIndicesToWriteTo);
        }
        else if (charInfoDictionary.TryGetValue(keywordMatch,
            out lastCharacterDesignated))
        {
            if (insideChoice)
            {
                eInfo.GetLastChoice().CharacterImage = lastCharacterDesignated.Sprite;
                eInfo.GetLastChoice().CharacterName = lastCharacterDesignated.DisplayName;
            }
        }
        else
        {
            Debug.Log(string.Join(" ", charInfoDictionary.Keys));
            throw new Exception(keywordMatch + " did not match any base " +
                "keywords, comment, dialogue " +
                "or any known character unique names. Is this dialogue/ a " +
                "choice and missing the keyword " +
                DIALOGUE.ToString() + " ?");
        }
    }

    private void ResolveKeyword(BaseKeyword value, List<string> trimmedText, 
        ref EventInfo eInfo, ref CharacterInfo lastCharacterDesignated, 
        ref bool insideChoice, ref bool insideChoiceBranchDialogue,
        List<string> choiceDialogueIndicesToWriteTo)
    {
        switch (value)
        {
            // required, denotes the start of an event and its UNIQUE name
            case BaseKeyword.Event:
                EventCreation(trimmedText, out eInfo);
                break;
            // not required, just allows a branch to escape an event asap
            // makes the most recent branch point to "end" as next
            case BaseKeyword.EventEarlyEnd:
                AddEarlyExitsToDialogue(eInfo, insideChoice, choiceDialogueIndicesToWriteTo);
                BranchEnd(ref insideChoice, ref insideChoiceBranchDialogue);
                break;
            // required, denotes the end of an event
            case BaseKeyword.EventEnd:
                eInfo = EventCreationComplete(eInfo);
                break;
            case BaseKeyword.Choice:
                ChoiceCreation(eInfo, ref insideChoice);
                break;
            // required after all choice branches have finished their dialogue
            case BaseKeyword.ChoiceEnd:
                BranchEnd(ref insideChoice, ref insideChoiceBranchDialogue);
                ChoiceComplete(eInfo, ref insideChoice, ref insideChoiceBranchDialogue);
                break;
            // declares a branch in a choice, different from branch start which is the start
            // of a branch's specific dialogue
            case BaseKeyword.Branch:
                CreateBranch(eInfo, ref insideChoice, ref insideChoiceBranchDialogue);
                break;
            case BaseKeyword.BranchStart:
                // still need to tell the dialogue grabbing thing that we want to write 
                // to a specific choice branch :d
                CreateChoiceBranch(eInfo, trimmedText, ref insideChoice, 
                    ref insideChoiceBranchDialogue, choiceDialogueIndicesToWriteTo);
                break;
            // end of a branch's dialogue not the end of its declaration
            case BaseKeyword.BranchEnd:
                BranchEnd(ref insideChoice, ref insideChoiceBranchDialogue);
                break;
            // the reward for choosing a specific choice
            case BaseKeyword.Reward:
                // need to look at next two words -> next one needs to be a reward keyword
                // last word needs to be an integer amount to be claimed for the reward
                AddRewardToChoice(trimmedText, eInfo, insideChoice);
                break;
            case BaseKeyword.Trigger:
                AddTriggeredEventsToDialogue(eInfo, trimmedText, insideChoice, choiceDialogueIndicesToWriteTo);
                break;
            // immediately adds reward on branch being played
            case BaseKeyword.EventReward:
                break;
            case BaseKeyword.BackgroundChange:
                break;
            default:
                throw new NotSupportedException("BaseKeyword does not yet support this");
        }
    }

    private void AddEarlyExitsToDialogue(EventInfo eInfo, bool insideChoice, List<string> choiceDialogueIndicesToWriteTo)
    {
        if (insideChoice)
            eInfo.MultiEarlyExitWrite(choiceDialogueIndicesToWriteTo);
        else
            eInfo.GetLastDialogue().ExitsEvent = true;
    }

    private void AddTriggeredEventsToDialogue(EventInfo eInfo, List<string> trimmedText, 
        bool insideChoice, List<string> choiceDialogueIndicesToWriteTo)
    {
        var eventName = trimmedText[1].ToLowerInvariant().Trim();
        if (insideChoice)
        {
            eInfo.MultiEventTriggerWrite(choiceDialogueIndicesToWriteTo, eventName);
        }
        else
        {
            // obviously the trigger needs a second word: the name of the event getting triggered
            eInfo.GetLastDialogue().DirectlyTriggeredEvents.Add(eventName);
        }

    }

    // placeholder in case this should do something at some point
    // branchend keyword is no longer doing anything though
    private static void BranchEnd(ref bool insideChoice, ref bool insideChoiceBranchDialogue)
    {
        return;
    }

    private void ChoiceCreation(EventInfo eInfo, ref bool insideChoice)
    {
        insideChoice = true;

        eInfo.AddChoice(new ChoiceInfo(eInfo.BranchID.ToString()));
        // need to add this choice's branch name to whatever the previous branch was
        eInfo.SetNextBranch(eInfo.GetLastDialogue().UniqueName, 
            eInfo.GetLastChoice().UniqueName);
    }

    // all actions that must be taken for choice_end
    private void ChoiceComplete(EventInfo eInfo, 
        ref bool insideChoice, ref bool insideChoiceBranchDialogue)
    {
        insideChoice = false;
        insideChoiceBranchDialogue = false;
        // also create next dialogue here and point choices to that dialogue
        var choice = eInfo.GetLastChoice();
        eInfo.AddDialogue(new DialogueInfo(eInfo.BranchID.ToString(),
                (string nextE) => { }, characterDictionary));
        var nextBranch = eInfo.GetLastDialogue().UniqueName;
        foreach (var ch in choice.ChoiceDialogueDictionary.Values)
        {
            eInfo.SetNextBranch(ch, nextBranch);
        }
    }

    private static void AddRewardToChoice(List<string> trimmedText, 
        EventInfo eInfo, bool insideChoice)
    {
        if (!insideChoice)
        {
            throw new Exception("can only provide normal rewards inside a choice" +
                " declaration");
        }
        if (trimmedText.Count < 3)
        {
            throw new Exception("reward command must specify the reward type and amount");
        }
        var rewardKey = trimmedText[1].ToLowerInvariant().Trim();
        if (REWARD_KEYWORDS.TryGetValue(rewardKey, out RewardKeyword rewardType))
        {
            // failure is desired if it doesnt work
            var rewardAmount = int.Parse(trimmedText[2].ToLowerInvariant().Trim());
            eInfo.GetLastChoice().AddReward(rewardType, rewardAmount);
        }
        else
        {
            throw new Exception(
                "reward key given is not in reward dictionary: " + rewardKey);
        }
    }

    private void CreateChoiceBranch(EventInfo eInfo, 
        List<string> trimmedText,
        ref bool insideChoice, ref bool insideChoiceBranchDialogue,
        List<string> choiceDialogueIndicesToWriteTo)
    {
        // closes the choice declaration here
        if (!insideChoice)
        {
            throw new Exception("must be in a choice to create choice branches");
        }
        // gets flipped to true for choice declaration completion as the next
        // dialogues must be the dialogues associated with this choice
        insideChoiceBranchDialogue = true;
        ChoiceDeclarationComplete(eInfo);

        insideChoice = true;
        trimmedText.PopFront();
        choiceDialogueIndicesToWriteTo.Clear();
        var choice = eInfo.GetLastChoice();
        foreach (var str in trimmedText)
        {
            // actual index is 1 less
            var index = int.Parse(str) - 1;
            if (choice.ChoiceDialogueDictionary.TryGetValue(index, out string name))
            {
                choiceDialogueIndicesToWriteTo.Add(name);
            }
            else
            {
                throw new Exception(index + " not found in choice dictionary");
            }
        }

    }

    private static void CreateBranch(EventInfo eInfo,
        ref bool insideChoice, ref bool insideChoiceBranchDialogue)
    {
        insideChoice = true;
        insideChoiceBranchDialogue = false;
        ChoiceInfo infoToModify = eInfo.GetLastChoice();
        infoToModify.AddChoice("");
    }

    private EventInfo EventCreationComplete(EventInfo eInfo)
    {
        if (eventDictionary.ContainsKey(eInfo.UniqueName))
        {
            throw new Exception("cannot add duplicate event name to event " +
                "dictionary: " + eInfo.UniqueName);
        }
        eInfo.GetLastDialogue().NextBranch = EventInfo.LAST_BRANCH;
        eInfo.GetLastDialogue().ExitsEvent = true;
        eventDictionary.Add(eInfo.UniqueName, eInfo);
        eInfo = null;
        return eInfo;
    }

    private void EventCreation(List<string> trimmedText, 
        out EventInfo eInfo)
    {
        if (trimmedText.Count < 2)
        {
            throw new Exception("event needs a name on declaration line");
        }
        eInfo = new EventInfo(trimmedText[1].ToLowerInvariant().Trim(), characterDictionary);
    }

    private void ChoiceDeclarationComplete(EventInfo eInfo)
    {
        if (!eInfo.GetLastChoice().IsFilledOut())
        {
            throw new NotImplementedException("choice not completely filled out with prompt: " +
                eInfo.GetLastChoice().Prompt);
        }
        var choice = eInfo.GetLastChoice();
        if (choice.ChoiceDialogueDictionary.Keys.Count == choice.Choices.Count)
            return;
        for (var i = 0; i < choice.Choices.Count; i++)
        {
            eInfo.AddDialogue(new DialogueInfo(eInfo.BranchID.ToString(),
                (string nextE) => { }, characterDictionary));
            choice.AddChoiceDialogueName(i, eInfo.GetLastDialogue().UniqueName);
        }
        
    }

    private bool ReadInContinues(List<string> trimmedText)
    {
        if (trimmedText.Count < 1)
        {
            return true;
        }
        if (trimmedText.First()[0] == COMMENT)
        {
            return true;
        }
        if (string.IsNullOrWhiteSpace(trimmedText.First()) ||
            string.IsNullOrEmpty(trimmedText.First()))
        {
            return true;
        }
        return false;
    }

    private void AddToDialogue(List<string> trimmedText,
        CharacterInfo lastCharacterDesignated,
        DialogueInfo dInfo
        )
    {
        if (lastCharacterDesignated == null)
        {
            Debug.LogError("no character designated for dialogue");
            return;
        }
        // remove that key character beginning if it was accidentally attached to 
        // next word
        string dialogueLine = ExtractDialogue(trimmedText);
        dInfo.AddDialogue(dialogueLine, lastCharacterDesignated.UniqueName);
    }

    private static string ExtractDialogue(List<string> trimmedText)
    {
        if (trimmedText[0].Length > 1) 
            trimmedText[0].Substring(1);
        else 
            trimmedText.PopFront();
        return string.Join(" ", trimmedText.ToArray());
    }

    private void CreateCharDictionary(List<CharacterInfo> characterList,
        out Dictionary<string, Tuple<string, Sprite>> charDictionary,
        out Dictionary<string, CharacterInfo> charInfoDictionary)
    {
        charDictionary = new Dictionary<string, Tuple<string, Sprite>>();
        charInfoDictionary = new Dictionary<string, CharacterInfo>();
        foreach (var character in characterList)
        {
            character.UniqueName = character.UniqueName.ToLowerInvariant();
            if (charDictionary.ContainsKey(character.UniqueName))
            {
                throw new Exception("characters must have unique names: " +
                    character.UniqueName);
            }
            charDictionary.Add(character.UniqueName,
                new Tuple<string, Sprite>(character.DisplayName, character.Sprite));
            charInfoDictionary.Add(character.UniqueName, character);
        }
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DialoguePrefab = Instantiate(dialoguePrefab);
        ChoicePrefab = Instantiate(choicePrefab);
        CreateCharDictionary(characterList,
            out characterDictionary, out charInfoDictionary);
        DialoguePrefab.GetComponent<DialogueController>().
            InitDictionaryOnly(characterDictionary);
        ReadInFiles(eventTextFiles);
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (useTestMode)
            foreach (var runnableEvent in eventDictionary.Values)
            {
                eventController.RunEvent(runnableEvent);
            }
#endif
    }

    void Update()
    {
        // update will actually maybe wanna do something?
        // will have to wait for event controller to give up control of 
        // the dialogue box when we have events queued up
        // but likely can just store a queue of events that are completely
        // ready right now and run them when event controller calls event
        // complete
    }
}
