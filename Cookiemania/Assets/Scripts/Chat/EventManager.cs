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

    public static EventManager Instance { get; private set; }

    public GameObject DialoguePrefab { get; private set; }

    public GameObject ChoicePrefab { get; private set; }

    public void EventComplete(string eventName, List<Tuple<RewardKeyword, int>> rewards)
    {
        // not sure what else we'd do here
        if (PlayerData.Player == null)
        {
            Debug.LogError("player data must exist to complete events");
            return;
        }
#if UNITY_EDITOR
        if (useTestMode)
            PlayerData.Player.PrintChoicesMade();
#endif
        DistributeRewards(rewards);
    }

    public void ChoiceMade(string eventName, string choicePrompt, string choiceMade)
    {
        if (PlayerData.Player == null)
        {
            Debug.LogError("player data must exist to view choices");
            return;
        }

        if (HasEvent(eventName))
        {
            if (PlayerData.Player.EventChoicesMade.TryGetValue(
                eventName, out List<Tuple<string, string>> addTo))
            {
                addTo.Add(new Tuple<string, string>(choicePrompt, choiceMade));
            }
            else
            {
                var list = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(choicePrompt, choiceMade)
                };
                PlayerData.Player.EventChoicesMade.Add(eventName, list);
            }
        }
        else
        {
            throw new Exception("event " + eventName + " not found");
        }
    }


    
    // in case something outside the event system wants to trigger an event, or 
    // if the event controller wants to trigger an event
    public bool TriggerEvent(string eventName)
    {
        if (eventDictionary.TryGetValue(eventName, out EventInfo eventInfo))
        {
            if (eventInfo.EventListening)
            {
                eventController.RunEvent(eventInfo);
                return true;
            }
        }
        return false;
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
        if (PlayerData.Player == null)
        {
            Debug.LogError("player data must exist for reward distribution");
            return;
        }

        foreach (var reward in rewards)
        {
            switch(reward.Item1)
            {
                case RewardKeyword.Money:
                    PlayerData.Player.money += reward.Item2;
                    break;
                case RewardKeyword.Morality:
                    PlayerData.Player.morality += reward.Item2;
                    break;
                default:
                    Debug.LogError("unhandled reward keyword for reward" +
                        " distribution: " + reward);
                    break;
            }
        }
    }

    private void ReadInFiles(List<TextAsset> textAssets)
    {
        char[] toTrim = { '\t' };
        foreach (var asset in textAssets)
        {
            EventParsingInfo parsingInfo = new EventParsingInfo();
            parsingInfo.MaxChoices = choiceLimit;
            foreach (var text in asset.text.Split('\n'))
            {
                parsingInfo.TrimmedLine = text.Trim(toTrim).Split(' ').ToList();
                if (ReadInContinues(parsingInfo.TrimmedLine))
                {
                    continue;
                }
                ParseDialogueOrKeyword(ref parsingInfo, charInfoDictionary);
            }
        }

    }

    private static void ParseDialogueOrKeyword(ref EventParsingInfo parsingInfo, 
        Dictionary<string, CharacterInfo> charDictionary)
    {
        // this first one needs to be lowercased and trimmed
        parsingInfo.TrimmedLine[0] = 
            parsingInfo.TrimmedLine[0].ToLowerInvariant().Trim();
        if (parsingInfo.TrimmedLine[0][0] == DIALOGUE)
        {
            ParseDialogueLine(ref parsingInfo);
        }
        else
        {
            ParsePossibleKeyword(ref parsingInfo, charDictionary);
        }
    }

    private static void ParseDialogueLine(ref EventParsingInfo parsingInfo)
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

    private static void ParsePossibleKeyword(ref EventParsingInfo parsingInfo, 
        Dictionary<string, CharacterInfo> charDictionary)
    {
        var keywordMatch = parsingInfo.TrimmedLine.First();
        if (BASE_KEYWORDS.TryGetValue(
            keywordMatch, out BaseKeyword keyword))
        {
            ResolveKeyword(keyword, ref parsingInfo);
        }
        else if (charDictionary.TryGetValue(keywordMatch,
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
            Debug.Log(string.Join(" ", charDictionary.Keys));
            throw new Exception(keywordMatch + " did not match any base " +
                "keywords, comment, dialogue " +
                "or any known character unique names. Is this dialogue/ a " +
                "choice and missing the keyword " +
                DIALOGUE.ToString() + " ?");
        }
    }

    private static void ResolveKeyword(BaseKeyword baseKey, ref EventParsingInfo parsingInfo)
    {
        if (KeywordActions.TryGetValue(baseKey,
            out ActionRef<EventParsingInfo> value))
            value.Invoke(ref parsingInfo);
        else
        {
            throw new NotSupportedException("the Action dictionary does not support this");
        }
    }

    private static bool ReadInContinues(List<string> trimmedText)
    {
        if (trimmedText.Count < 1)
        {
            return true;
        }
        if (trimmedText[0][0] == COMMENT)
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

    private static void AddToDialogue(string extractedDialogue,
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
}
