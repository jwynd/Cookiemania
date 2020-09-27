using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public readonly Dictionary<string, BaseKeyword> BASE_KEYWORDS = new Dictionary<string, BaseKeyword>{
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
    };

    // need to perform validity checks to ensure trigger_keywords + base_keywords don't have
    // overlapping keywords e.g. "end" can't be a keyword in both dictionaries
    // will also need to check character's unique names also don't overlap with either of these
    // dictionaries keywords
    public readonly Dictionary<string, TriggerKeyword> TRIGGER_KEYWORDS = new Dictionary<string, TriggerKeyword>{
        { "end" , TriggerKeyword.EventEnd },
        { "ends" , TriggerKeyword.EventEnd },
        { "start" , TriggerKeyword.EventStart },
        { "starts" , TriggerKeyword.EventStart },
        { "money" , TriggerKeyword.Money },
        { "day" , TriggerKeyword.Day },
        { "days" , TriggerKeyword.Day },
        { "morality" , TriggerKeyword.Morality },
        { "direct", TriggerKeyword.DirectTrigger },
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
        DirectTrigger
    }

    [Serializable]
    public class CharInfo
    {
        public Sprite sprite;
        public string displayName;
        public string uniqueName;
    }

    [SerializeField]
    private GameObject dialoguePrefab = null;
    [SerializeField]
    private GameObject choicePrefab = null;
    [SerializeField]
    [Tooltip("The text files that contain events to be registered")]
    private List<TextAsset> eventTextFiles = new List<TextAsset>();
    [SerializeField]
    private List<CharInfo> characterDictionary = new List<CharInfo>();

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
