using General_Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private const char commentKeyword = '#';
    private const char dialogueKeyword = '>';
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
        public string uniqueName;

        public CharInfo(Sprite sprite, string displayName, string uniqueName)
        {
            this.sprite = sprite;
            this.displayName = displayName;
            this.uniqueName = uniqueName;
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

        public DialogueInfo(List<Tuple<string, string>> dialogues,
            OnComplete runOnComplete, 
            Dictionary<string, Tuple<string, Sprite>> characterDictionary)
        {
            this.dialogues = dialogues;
            this.runOnComplete = runOnComplete;
            this.characterDictionary = characterDictionary;
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
        public Sprite charImage;
        public string charName;
        public string choicePrompt;
        public List<string> choices;
        public List<List<Tuple<RewardKeyword, int>>> rewards;
        public OnChoiceComplete onComplete;

        public ChoiceInfo(Sprite charImage, string charName, string choicePrompt,
            List<string> choices, List<List<Tuple<RewardKeyword, int>>> rewards, 
            OnChoiceComplete onComplete, int choiceLimit = 4)
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
            this.choices = new List<string>();
            this.rewards = new List<List<Tuple<RewardKeyword, int>>>();
            foreach(var choice in choices)
            {
                this.choices.Add(choice);
            }
            foreach(var reward in rewards)
            {
                this.rewards.Add(reward);
            }
        }
    }

    [Serializable]
    public class EventInfo
    {
        public Tuple<TriggerKeyword, int> triggeringAction;
        // set it to false when event runs and direct trigger was not used
        public bool eventListening = true;
        public List<DialogueInfo> dialogues;
        public List<ChoiceInfo> choices;
        // the tuple is --> true means its just a dialogue, false is a choice
        // and the int is its index in the list
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
    private List<CharInfo> characterDictionary = new List<CharInfo>();

    public static EventManager Instance { get; private set; }

    private void ReadInFiles(List<TextAsset> textAssets)
    {
        char[] toTrim = { '\t' };
        CharInfo lastCharacterDesignated = characterDictionary.FirstOrDefault();
        foreach (var asset in textAssets)
        {
            var texts = asset.text.Split('\n');
            foreach (var text in texts)
            {
                var trimmedText = text.Trim(toTrim).Split(' ').ToList();
                if (trimmedText.Count < 1)
                {
                    continue;
                }
                if (trimmedText[0][0] == commentKeyword)
                {
                    Debug.Log("skipped " + trimmedText[0]);
                    continue;
                }
                if (trimmedText[0][0] == dialogueKeyword)
                {
                    RegisterDialogue(trimmedText, lastCharacterDesignated);
                    continue;
                }
                foreach (var word in trimmedText)
                {
                    Debug.Log(word);
                }
            }
        }
        
    }

    private void RegisterDialogue(List<string> trimmedText, CharInfo lastCharacterDesignated)
    {
        trimmedText.PopFront();
        var line = string.Join(" ", trimmedText.ToArray());
        Debug.Log(line);
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
        ReadInFiles(eventTextFiles);
    }

    void Update()
    {
        // need to check for triggers when morality / days / money changes NOT
        // in update loop ---> should be entirely unnecessary
        // so obv use c#'s event
    }
}
