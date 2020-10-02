using General_Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private const char commentKeyword = '#';
    private const char dialogueKeyword = '>';

    // no uppercase letters in any of the keywords allowed
    public readonly Dictionary<string, BaseKeyword> BASE_KEYWORDS =
        new Dictionary<string, BaseKeyword>
    {
        { "event" , BaseKeyword.Event },
        { "events", BaseKeyword.Event },
        { "choice", BaseKeyword.Choice },
        { "choices", BaseKeyword.Choice },
        { "branch", BaseKeyword.Branch },
        { "branches", BaseKeyword.Branch },
        { "event_end", BaseKeyword.EventEnd },
        { "event_early_end", BaseKeyword.EventEarlyEnd },
        { "events_early_end", BaseKeyword.EventEarlyEnd },
        { "events_end", BaseKeyword.EventEnd },
        { "choice_end", BaseKeyword.ChoiceEnd },
        { "choices_end", BaseKeyword.ChoiceEnd },
        { "branch_end", BaseKeyword.BranchEnd },
        { "branches_end", BaseKeyword.BranchEnd },
        { "branch_start", BaseKeyword.BranchStart },
        { "branches_start", BaseKeyword.BranchStart },
        { "trigger", BaseKeyword.Trigger },
        { "reward", BaseKeyword.Reward },
    };

    // reads second word after trigger to figure out what to do next
    // if trigger trigger then event doesnt listen for anything and is called 
    // by other events only
    public readonly Dictionary<string, TriggerKeyword> TRIGGER_KEYWORDS =
        new Dictionary<string, TriggerKeyword>
    {
        { "end" , TriggerKeyword.EventEnd },
        { "ends" , TriggerKeyword.EventEnd },
        { "start" , TriggerKeyword.EventStart },
        { "starts" , TriggerKeyword.EventStart },
        { "money" , TriggerKeyword.Money },
        { "day" , TriggerKeyword.Day },
        { "days" , TriggerKeyword.Day },
        { "morality" , TriggerKeyword.Morality },
        { "trigger", TriggerKeyword.DirectTrigger },
    };

    // reads second word after reward
    public readonly Dictionary<string, RewardKeyword> REWARD_KEYWORDS =
        new Dictionary<string, RewardKeyword>
    {
        { "morality" , RewardKeyword.Morality },
        { "money" , RewardKeyword.Money },
    };

    public enum BaseKeyword
    {
        Event,
        Choice,
        Branch,
        EventEnd,
        // only used for branches that want to exit the event
        // without going to some kind of shared dialogue continuation
        // after the branches are closed up
        EventEarlyEnd,
        ChoiceEnd,
        BranchEnd,
        BranchStart,
        Trigger,
        Reward,
    }

    public enum TriggerKeyword
    {
        EventStart,
        EventEnd,
        Money,
        Day,
        Morality,
        // e.g. a quest will directly trigger it and does not need to 
        // be registered
        DirectTrigger,
    }

    public enum RewardKeyword
    {
        Money,
        Morality,
    }

    [Serializable]
    public class CharInfo
    {
        public Sprite sprite;
        public string displayName;
        [Tooltip("The unique name is case insensitive: e.g. BOSS and boss are the same")]
        public string uniqueName;

        public CharInfo(Sprite sprite, string displayName, string uniqueName)
        {
            this.sprite = sprite;
            this.displayName = displayName;
            this.uniqueName = uniqueName.ToLowerInvariant();
        }
    }

    [HideInInspector]
    public delegate void OnComplete();
    [HideInInspector]
    public delegate void OnChoiceComplete(int chosenValue);


    [Serializable]
    public class DialogueInfo
    {
        // item 1 is the characters unique name, item 2 is their line
        public List<Tuple<string, string>> dialogues;
        public OnComplete runOnComplete;
        public Dictionary<string, Tuple<string, Sprite>> characterDictionary;
        public string nextEvent;
        public string uniqueName;

        public DialogueInfo(
            string uniqueName,
            OnComplete runOnComplete,
            Dictionary<string, Tuple<string, Sprite>> characterDictionary,
            string nextEvent = "",
            List<Tuple<string, string>> dialogues = null)
        {
            this.dialogues = dialogues != null ?
                dialogues : new List<Tuple<string, string>>();
            this.runOnComplete = runOnComplete;
            this.characterDictionary = characterDictionary;
            this.nextEvent = nextEvent;
            this.uniqueName = uniqueName;
        }

        // if no unique name is given, argument will be provided as most recent talking
        // character
        public void AddDialogue(string dialogue, string charUniqueName = "")
        {
            if (charUniqueName == "")
            {
                charUniqueName = dialogues.Last().Item1;
            }
            dialogues.Add(new Tuple<string, string>(charUniqueName, dialogue));
        }
    }

    [Serializable]
    public class ChoiceInfo
    {
        public string uniqueName;
        public Sprite charImage;
        public string charName;
        public string choicePrompt;
        public string nextEvent;
        // on each branch 1 etc declaration makes a new choice with empty string
        // and a new list of specific rewards in the rewards list
        // next line fills in text for declaration
        // reward lines fill in the reward list
        public List<string> choices;
        public List<List<Tuple<RewardKeyword, int>>> rewards;
        public OnChoiceComplete onComplete;


        public ChoiceInfo(string uniqueName)
        {
            choices = new List<string>();
            rewards = new List<List<Tuple<RewardKeyword, int>>>();
            charName = "";
            choicePrompt = "";
            nextEvent = "";
            this.uniqueName = uniqueName;
        }

        public bool IsFilledOut()
        {
            return charImage != null && charName != "" &&
                choicePrompt != "" && nextEvent != "" && choices.Count >= 1 &&
                rewards.Count >= 1 && onComplete != null;
        }

        public ChoiceInfo(Sprite charImage, string charName, string choicePrompt,
            List<string> choices, List<List<Tuple<RewardKeyword, int>>> rewards,
            OnChoiceComplete onComplete, string nextEvent, int choiceLimit = 4)
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
            this.charImage = charImage;
            this.charName = charName;
            this.choicePrompt = choicePrompt;
            this.onComplete = onComplete;
            this.nextEvent = nextEvent;
            this.choices = new List<string>();
            this.rewards = new List<List<Tuple<RewardKeyword, int>>>();
            foreach (var choice in choices)
            {
                this.choices.Add(choice);
            }
            foreach (var reward in rewards)
            {
                this.rewards.Add(reward);
            }
        }
    }

    [Serializable]
    public class EventInfo
    {
        public Tuple<TriggerKeyword, int> triggeringAction;
        public string uniqueName = "";
        // set it to false when event runs and direct trigger was not used
        public bool eventListening = true;
        private List<DialogueInfo> dialogues = new List<DialogueInfo>();
        private List<ChoiceInfo> choices = new List<ChoiceInfo>();

        public EventInfo(string uniqueName)
        {
            this.uniqueName = uniqueName;
        }

        public void AddDialogue(DialogueInfo dInfo)
        {
            dialogues.Add(dInfo);
            BranchID++;
        }

        public void AddChoice(ChoiceInfo cInfo)
        {
            choices.Add(cInfo);
            BranchID++;
        }

        public ChoiceInfo GetLastChoice()
        {
            return choices.Last();
        }

        public DialogueInfo GetLastDialogue()
        {
            return dialogues.Last();
        }

        public int BranchID
        {
            get; private set;
        }
        // the tuple is --> true means its just a dialogue, false is a choice
        // and the int is its index in the list
        // NOTE: the OnComplete for each choices / dialogues should be 
        // starting an event with a string in the branchingDictionary
        // 
        public Dictionary<string, Tuple<bool, int>> branchingDictionary;
    }

    [SerializeField]
    private int choiceLimit = 4;
    [SerializeField]
    private GameObject dialoguePrefab = null;
    [SerializeField]
    private GameObject choicePrefab = null;
    [SerializeField]
    [Tooltip("The text files that contain events to be registered")]
    private List<TextAsset> eventTextFiles = new List<TextAsset>();
    [SerializeField]
    private List<CharInfo> characterList = new List<CharInfo>();
    private Dictionary<string, Tuple<string, Sprite>> characterDictionary =
        new Dictionary<string, Tuple<string, Sprite>>();
    private Dictionary<string, CharInfo> charInfoDictionary = 
        new Dictionary<string, CharInfo>();
    private Dictionary<string, EventInfo> eventDictionary = 
        new Dictionary<string, EventInfo>();

    public static EventManager Instance { get; private set; }

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
            CharInfo lastCharacterDesignated = null;
            bool insideChoice = false;
            bool insideBranchDeclaration = false;
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
                if (trimmedText.First()[0] == dialogueKeyword)
                {
                    // only unset this when we hit the choice_end or branch_start keywords
                    if (insideChoice)
                    {
                        // need to go down the prio list --> if prompt filled go to last 
                        // choice and replace the string there
                        // if that string is not "" throw an error cuz choices only get 
                        // one line of text.
                        throw new NotImplementedException();
                    }
                    else
                    {
                        RegisterDialogue(trimmedText, lastCharacterDesignated,
                            eInfo.GetLastDialogue());
                    }
                    continue;
                }

                var keywordMatch = trimmedText.First();
                if (BASE_KEYWORDS.TryGetValue(keywordMatch, out BaseKeyword keyword))
                {
                    ResolveKeyword(keyword, trimmedText, ref eInfo,
                        ref lastCharacterDesignated, ref insideChoice, 
                        ref insideBranchDeclaration);
                }
                else if (charInfoDictionary.TryGetValue(keywordMatch, 
                    out lastCharacterDesignated))
                {
                    continue;
                }
                else
                {
                    Debug.Log(string.Join(" ", charInfoDictionary.Keys));
                    throw new Exception(keywordMatch + " did not match any base " +
                        "keywords, comment, dialogue " +
                        "or any known character unique names. Is this dialogue/ a " +
                        "choice and missing the keyword " + 
                        dialogueKeyword.ToString() + " ?");
                }
            }
        }

    }

    private void ResolveKeyword(BaseKeyword value, List<string> trimmedText, 
        ref EventInfo eInfo, ref CharInfo lastCharacterDesignated, 
        ref bool insideChoice, ref bool insideBranchDeclaration)
    {
        switch (value)
        {
            // required, denotes the start of an event and its UNIQUE name
            case BaseKeyword.Event:
                if (trimmedText.Count < 2)
                {
                    throw new Exception("event needs a name on declaration line");
                }
                eInfo = new EventInfo(trimmedText[1]);
                DialogueInfo dInfo = new DialogueInfo(eInfo.BranchID.ToString(),
                    () => { }, characterDictionary);
                // adding a default dialogue to event info for first dialogue
                eInfo.AddDialogue(dInfo);
                break;
            // not required, just allows a branch to escape an event asap
            // makes the most recent branch point to "end" as next
            case BaseKeyword.EventEarlyEnd:
                break;
            // required, denotes the end of an event
            case BaseKeyword.EventEnd:
                if (eventDictionary.ContainsKey(eInfo.uniqueName))
                {
                    throw new Exception("cannot add duplicate event name to event " +
                        "dictionary: " + eInfo.uniqueName);
                }
                eventDictionary.Add(eInfo.uniqueName, eInfo);
                eInfo = null;
                break;
            case BaseKeyword.Choice:
                insideChoice = true;
                eInfo.AddChoice(new ChoiceInfo(eInfo.BranchID.ToString()));
                break;
            // required after branch declaration of 
            case BaseKeyword.ChoiceEnd:
                ChoiceDeclarationComplete(ref insideChoice, eInfo);
                break;
            // declares a branch in a choice, different from branch start which is the start
            // of a branch's specific dialogue
            case BaseKeyword.Branch:
                ChoiceInfo infoToModify = eInfo.GetLastChoice();
                infoToModify.choices.Add("");
                infoToModify.rewards.Add(new List<Tuple<RewardKeyword, int>>());
                insideBranchDeclaration = true;
                break;
            case BaseKeyword.BranchStart:
                insideBranchDeclaration = false;
                if (insideChoice)
                {
                    ChoiceDeclarationComplete(ref insideChoice, eInfo);
                }
                eInfo.AddDialogue(new DialogueInfo(eInfo.BranchID.ToString(),
                    () => { }, characterDictionary));
                break;
            // end of a branch's dialogue not the end of its declaration
            case BaseKeyword.BranchEnd:
                break;
            // the reward for choosing a specific choice
            case BaseKeyword.Reward:
                break;
            case BaseKeyword.Trigger:
                break;
            default:
                throw new NotSupportedException("BaseKeyword does not yet support this");
        }
    }

    private void ChoiceDeclarationComplete(ref bool insideChoice, EventInfo eInfo)
    {
        insideChoice = false;
        if (!eInfo.GetLastChoice().IsFilledOut())
        {
            throw new NotImplementedException("choice not completely filled out with prompt: " +
                eInfo.GetLastChoice().choicePrompt);
        }
    }

    private bool ReadInContinues(List<string> trimmedText)
    {
        if (trimmedText.Count < 1)
        {
            return true;
        }
        if (trimmedText.First()[0] == commentKeyword)
        {
            return true;
        }
        if (String.IsNullOrWhiteSpace(trimmedText.First()) ||
            String.IsNullOrEmpty(trimmedText.First()))
        {
            return true;
        }
        return false;
    }

    private void RegisterDialogue(List<string> trimmedText,
        CharInfo lastCharacterDesignated,
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
        if (trimmedText[0].Length > 1)
        {
            trimmedText[0].Substring(1);
        }
        else
        {
            trimmedText.PopFront();
        }
        var dialogueLine = string.Join(" ", trimmedText.ToArray());
        dInfo.AddDialogue(dialogueLine, lastCharacterDesignated.uniqueName);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        CreateCharDictionary(characterList, out characterDictionary, out charInfoDictionary);
        ReadInFiles(eventTextFiles);
    }

    private void CreateCharDictionary(List<CharInfo> characterList,
        out Dictionary<string, Tuple<string, Sprite>> charDictionary,
        out Dictionary<string, CharInfo> charInfoDictionary)
    {
        charDictionary = new Dictionary<string, Tuple<string, Sprite>>();
        charInfoDictionary = new Dictionary<string, CharInfo>();
        foreach (var character in characterList)
        {
            character.uniqueName = character.uniqueName.ToLowerInvariant();
            if (charDictionary.ContainsKey(character.uniqueName))
            {
                throw new Exception("characters must have unique names: " + 
                    character.uniqueName);
            }
            charDictionary.Add(character.uniqueName,
                new Tuple<string, Sprite>(character.displayName, character.sprite));
            charInfoDictionary.Add(character.uniqueName, character);
        }
    }

    void Update()
    {
        // need to check for triggers when morality / days / money changes NOT
        // in update loop ---> should be entirely unnecessary
        // so obv use c#'s event
    }
}
