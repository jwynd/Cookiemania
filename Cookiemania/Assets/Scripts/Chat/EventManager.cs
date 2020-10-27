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

    // the arguments for action are: the current event info, the trimmed text, 
    // the list of indices to write to and a tuple
    // with the inChoice and inDialogueBranch for choice bools


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

    public bool HasEvent(string eventName)
    {
        return eventDictionary.ContainsKey(eventName);
    }

    public bool AddEvent(EventInfo eventInfo)
    {
        if (HasEvent(eventInfo.UniqueName))
            return false;
        eventDictionary.Add(eventInfo.UniqueName, eventInfo);
        return true;
    }

    public void DistributeRewards(List<Tuple<RewardKeyword, int>> rewards)
    {
        Debug.LogWarning("need to add reward distribution");
    }

    private void ReadInFiles(List<TextAsset> textAssets)
    {
        char[] toTrim = { '\t' };
        foreach (var asset in textAssets)
        {
            EventParsingInfo parsingInfo = new EventParsingInfo();

            // will swap back to foreach loop if i dont end up needing the index
            foreach (var text in asset.text.Split('\n'))
            {
                parsingInfo.TrimmedLine = text.Trim(toTrim).Split(' ').ToList();
                if (ReadInContinues(parsingInfo.TrimmedLine))
                {
                    continue;
                }
                ParseDialogueOrKeyword(ref parsingInfo);
            }
        }

    }

    private void ParseDialogueOrKeyword(ref EventParsingInfo parsingInfo)
    {
        // this first one needs to be lowercased and trimmed
        parsingInfo.TrimmedLine[0] = parsingInfo.TrimmedLine.First().ToLowerInvariant().Trim();
        if (parsingInfo.TrimmedLine.First()[0] == DIALOGUE)
        {
            ParseDialogueLine(ref parsingInfo);
        }
        else
        {
            ParsePossibleKeyword(ref parsingInfo);
        }
    }

    private void ParseDialogueLine(ref EventParsingInfo parsingInfo)
    {
        var textLine = ExtractDialogue(parsingInfo.TrimmedLine);
        // if not in choice
        if (!parsingInfo.IsChoiceIsChoiceDialogue.Item1)
        {
            AddToDialogue(textLine, parsingInfo.CharacterInfo,
                parsingInfo.EventInfo.GetLastDialogue());
            return;
        }
        // if in choice declaration
        if (!parsingInfo.IsChoiceIsChoiceDialogue.Item2)
        {
            var choice = parsingInfo.EventInfo.GetLastChoice();
            if (choice.Prompt == "")
                choice.Prompt = textLine;
            else
                choice.Choices[choice.Choices.Count - 1] = textLine;
        }
        // if in choice dialogue branch
        else
        {
            // need to make sure we fill the correct dialogue, not just 
            // the most recent one
            parsingInfo.EventInfo.MultiDialogueWrite(
                parsingInfo.ChoiceDialoguesToMultiWrite, 
                textLine,
                parsingInfo.CharacterInfo.UniqueName);
        }
    }

    private void ParsePossibleKeyword(ref EventParsingInfo parsingInfo)
    {
        var keywordMatch = parsingInfo.TrimmedLine.First();
        if (BASE_KEYWORDS.TryGetValue(
            keywordMatch, out BaseKeyword keyword))
        {
            ResolveKeyword(keyword, ref parsingInfo);
        }
        else if (charInfoDictionary.TryGetValue(keywordMatch,
            out parsingInfo.CharacterInfo))
        {
            if (parsingInfo.IsChoiceIsChoiceDialogue.Item1)
            {
                parsingInfo.EventInfo.GetLastChoice().CharacterImage = 
                    parsingInfo.CharacterInfo.Sprite;
                parsingInfo.EventInfo.GetLastChoice().CharacterName = 
                    parsingInfo.CharacterInfo.DisplayName;
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

    private void ResolveKeyword(BaseKeyword baseKey, ref EventParsingInfo parsingInfo)
    {
        if (KeywordActions.TryGetValue(baseKey,
            out ActionRef<EventParsingInfo> value))
            value.Invoke(ref parsingInfo);
        else
        {
            throw new NotSupportedException("the Action dictionary does not yet support this");
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

    private void AddToDialogue(string extractedDialogue,
        CharacterInfo lastCharacterDesignated,
        DialogueInfo dInfo)
    {
        if (lastCharacterDesignated == null)
        {
            Debug.LogError("no character designated for dialogue");
            return;
        }
        dInfo.AddDialogue(extractedDialogue, lastCharacterDesignated.UniqueName);
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
