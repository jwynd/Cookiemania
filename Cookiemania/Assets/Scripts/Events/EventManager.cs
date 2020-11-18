using General_Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor.Events;
using UnityEngine;

using static Parsing_Utilities;

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

    [Serializable]
    public struct NameOverwrites
    {
        public string DialogueKeyword;
        public string UniqueName;

        public NameOverwrites(string keyword, string name)
        {
            DialogueKeyword = keyword;
            UniqueName = name;
        }
    }

    [SerializeField]
    [Tooltip("Keywords in the dialogue that should be replaced by a character's actual name\n" +
        "Basically anything that gets named by the player, like the player character's name\n" +
        "Each entry should be (keyword_to_be_replaced, character's_unique_name)")]
    private List<NameOverwrites> dialogueOverwrites = new List<NameOverwrites>();
    [SerializeField]
    private List<CharacterInfo> characterList = new List<CharacterInfo>();
    [SerializeField]
    private List<BackgroundInfo> backgroundList = new List<BackgroundInfo>();
    [SerializeField]
    private bool useTestMode = true;

    // these dictionaries are only initialized
    public ReadOnlyDictionary<string, Tuple<string, Sprite>> CharacterDictionary;
    private ReadOnlyDictionary<string, BackgroundInfo> backgroundDictionary;
    private ReadOnlyDictionary<string, EventInfo> eventDictionary;
    // value is the corresponding key entry in the character dictionary
    private ReadOnlyDictionary<string, string> dialogueKeywordToDisplayName;

    // mutable dictionary
    private Dictionary<TriggerKeyword, SortedList<int, List<EventInfo>>> listeningEvents =
        new Dictionary<TriggerKeyword, SortedList<int, List<EventInfo>>>();

    private int dialogueCharacterLimit = 140;

    public static EventManager Instance { get; private set; }

    public GameObject DialoguePrefab { get; private set; }

    public GameObject ChoicePrefab { get; private set; }

    public EmailController Email { get; private set; }

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

    // basically only used for player, cuz they're the only character 
    // that gets named / updated
    // pass null sprite to skip update of the sprite
    public int UpdateCharacterEntry(string newName,
        Sprite newSprite = null, string playerUID = "player")
    {
        if (CharacterDictionary == null)
        {
            return -2;
        }

        if (!CharacterDictionary.TryGetValue(playerUID, out Tuple<string, Sprite> value))
        {
            return -1;
        }
        var sprite = newSprite == null ? value.Item2 : newSprite;
        var newDict =
            new Dictionary<string, Tuple<string, Sprite>>(CharacterDictionary)
            {
                [playerUID] = new Tuple<string, Sprite>(newName, sprite)
            };
        CharacterDictionary = new ReadOnlyDictionary<string, Tuple<string, Sprite>>(newDict);
        // needs to be recreated here as we dont know if any overwrite got changed
        CreateOverwriteDictionary(this.dialogueOverwrites, CharacterDictionary);
        return 0;
    }

    public void SetEmailReference(EmailController emailController)
    {
        Email = emailController;
    }

    // searches the string for any instances of the dialogue keywords
    public string GetDialogueWithOverwrites(string line)
    {
        // not removing empty entries here e.g. "hello    what's up" 
        // wont have extra whitespace removed
        foreach (var key in dialogueKeywordToDisplayName.Keys)
        {
            if (dialogueKeywordToDisplayName.TryGetValue(key, out string value))
                line = line.Replace(key, value);
        }
        return line;
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
        else
        {
            Debug.LogError("couldn't find " + eventName);
            Debug.LogError("Dictionary entries: " + string.Join(", ", eventDictionary.Keys));
        }
        return false;
    }

    public bool HasEvent(string eventName)
    {
        return eventDictionary.ContainsKey(eventName);
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
                    PlayerData.Player.AddMoney(reward.Item2);
                    break;
                case RewardKeyword.Morality:
                    PlayerData.Player.AddMorality(reward.Item2);
                    break;
                case RewardKeyword.ShopLevel:
                    PlayerData.Player.shoplvl += reward.Item2;
                    break;
                case RewardKeyword.Week:
                    PlayerData.Player.week += reward.Item2;
                    break;
                default:
                    Debug.LogError("unhandled reward keyword for reward" +
                        " distribution: " + reward);
                    break;
            }
        }
    }

    private void ParseEventScripts(List<TextAsset> textAssets, 
        ReadOnlyDictionary<string, CharacterInfo> charInfoDictionary)
    {
        char[] toTrim = { '\t' };
        EventParsingInfo parsingInfo = new EventParsingInfo();
        foreach (var asset in textAssets)
        {
            parsingInfo.MaxChoices = choiceLimit;
            foreach (var text in asset.text.Split('\n'))
            {
                parsingInfo.TrimmedLine = text.Split(
                    new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (SkipLine(parsingInfo.TrimmedLine))
                {
                    continue;
                }
                ParseDialogueOrKeyword(ref parsingInfo, dialogueCharacterLimit,
                    charInfoDictionary, backgroundDictionary);
            }
        }
        eventDictionary = new ReadOnlyDictionary<string, EventInfo>
            (parsingInfo.EventInfos);
        VerifyDirectTriggerEvents(eventDictionary);
    }

    private void VerifyDirectTriggerEvents(
        ReadOnlyDictionary<string, EventInfo> eventDictionary)
    {
        foreach (var eventInfo in eventDictionary.Values)
        {
            foreach (var dialogue in eventInfo.GetAllDialogues(
                eventInfo.BranchingDictionary.Keys.ToList(), true))
            {
                foreach (var eventName in dialogue.DirectlyTriggeredEvents)
                {
                    if (!HasEvent(eventName))
                    {
                        Debug.LogError(eventName + " event not found for " +
                            "direct triggering as " +
                            "requested in event " + eventInfo.UniqueName);
                    }
                }
            }
        }
    }

    // you could argue ref is unnecessary, but these are static methods
    // and invokers need to understand that anything marked ref can 
    // change when going through this function
    private static void ParseDialogueOrKeyword(ref EventParsingInfo parsingInfo, 
        int charLimit,
        ReadOnlyDictionary<string, CharacterInfo> charDictionary,
        ReadOnlyDictionary<string, BackgroundInfo> bgDictionary)
    {
        // this first one needs to be lowercased and trimmed
        parsingInfo.TrimmedLine[0] = 
            parsingInfo.TrimmedLine[0].ToLowerInvariant().Trim();
        if (parsingInfo.TrimmedLine[0][0] == DIALOGUE)
        {
            ResolveDialogueLine(ref parsingInfo, charLimit);
        }
        else
        {
            ParseKeyword(ref parsingInfo, charDictionary, bgDictionary);
        }
    }

    private static void ResolveDialogueLine(ref EventParsingInfo parsingInfo, int charLimit)
    {
        var textLine = ExtractDialogue(ref parsingInfo.TrimmedLine, charLimit);
        // if not in choice
        if (!parsingInfo.IsChoiceIsChoiceDialogue.Item1)
        {
            AddToDialogue(textLine, parsingInfo.CharacterInfo,
                parsingInfo.BackgroundInfo?.Background,
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
                parsingInfo.CharacterInfo?.UniqueName, 
                parsingInfo.BackgroundInfo?.Background);
        }
    }

    private static void ParseKeyword(ref EventParsingInfo parsingInfo,
        ReadOnlyDictionary<string, CharacterInfo> charDictionary, 
        ReadOnlyDictionary<string, BackgroundInfo> bgDictionary)
    {
        var keywordMatch = parsingInfo.TrimmedLine.First();
        if (BASE_KEYWORDS.TryGetValue(
            keywordMatch, out BaseKeyword keyword))
        {
            ResolveBaseKeyword(ref parsingInfo, keyword);
        }
        else if (charDictionary.TryGetValue(keywordMatch,
            out CharacterInfo cInfo))
        {
            ResolveCharacterKeyword(ref parsingInfo, cInfo);
        }
        else if (bgDictionary.TryGetValue(keywordMatch,
            out BackgroundInfo bgInfo)) 
        {
            ResolveBackgroundKeyword(ref parsingInfo, bgInfo);
        }
        else
        {
            Debug.Log(string.Join(" ", charDictionary.Keys));
            throw new Exception(keywordMatch + " did not match any base " +
                "keywords, comment, dialogue " +
                "or any known character/bg unique names. Is this a dialogue" +
                "and missing the keyword " +
                DIALOGUE.ToString() + " ?");
        }
    }

    private static void ResolveBackgroundKeyword(ref EventParsingInfo parsingInfo, 
        BackgroundInfo bgInfo)
    {
        parsingInfo.BackgroundInfo = bgInfo;
        if (parsingInfo.IsChoiceIsChoiceDialogue.Item1 &&
            !parsingInfo.IsChoiceIsChoiceDialogue.Item2)
        {
            parsingInfo.EventInfo.GetLastChoice().Background =
                parsingInfo.BackgroundInfo.Background;
        }
    }

    private static void ResolveCharacterKeyword(ref EventParsingInfo parsingInfo,
        CharacterInfo cInfo)
    {
        parsingInfo.CharacterInfo = cInfo;
        if (parsingInfo.IsChoiceIsChoiceDialogue.Item1 && 
            !parsingInfo.IsChoiceIsChoiceDialogue.Item2)
        {
            parsingInfo.EventInfo.GetLastChoice().CharacterImage =
                parsingInfo.CharacterInfo.Sprite;
            parsingInfo.EventInfo.GetLastChoice().CharacterName =
                parsingInfo.CharacterInfo.DisplayName;
        }
    }

    private static void ResolveBaseKeyword(ref EventParsingInfo parsingInfo, BaseKeyword baseKey)
    {
        if (KeywordActions.TryGetValue(baseKey,
            out ActionRef<EventParsingInfo> value))
            value.Invoke(ref parsingInfo);
        else
        {
            throw new NotSupportedException("the Action dictionary does not support this");
        }
    }

    private static bool SkipLine(List<string> trimmedText)
    {
        if (trimmedText.Count < 1)
        {
            return true;
        }
        if (string.IsNullOrWhiteSpace(trimmedText.First()) ||
            string.IsNullOrEmpty(trimmedText.First()))
        {
            return true;
        }
        if (trimmedText[0][0] == COMMENT)
        {
            return true;
        }

        return false;
    }

    private static void AddToDialogue(string extractedDialogue,
        CharacterInfo lastCharacterDesignated,
        Sprite background,
        DialogueInfo dInfo)
    {
        if (lastCharacterDesignated == null)
        {
            Debug.LogError("no character designated for dialogue");
            return;
        }
        dInfo.AddDialogue(extractedDialogue, lastCharacterDesignated.UniqueName,
            background);
    }

    private static string ExtractDialogue(ref List<string> trimmedText, int charLimit)
    {
        if (trimmedText[0].Length > 1)
            trimmedText[0].Substring(1);
        else
            trimmedText.PopFront();
        var actualLine = string.Join(" ", trimmedText.ToArray());
        if (actualLine.Length > charLimit)
        {
            throw new Exception("dialogue line is too long");
        }
        return actualLine;
    }

    private ReadOnlyDictionary<string, CharacterInfo> CreateCharDictionaries(
        List<CharacterInfo> characterList)
    {
        var charDictionary = new Dictionary<string, Tuple<string, Sprite>>();
        var charInfoDictionary = new Dictionary<string, CharacterInfo>();
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
        this.CharacterDictionary = new ReadOnlyDictionary<string, Tuple<string, Sprite>>
            (charDictionary);
        CreateOverwriteDictionary(this.dialogueOverwrites, this.CharacterDictionary);
        return new ReadOnlyDictionary<string, CharacterInfo>
            (charInfoDictionary);
    }

    private void CreateOverwriteDictionary(List<NameOverwrites> overwrites, 
        ReadOnlyDictionary<string, Tuple<string, Sprite>> charDictionary)
    {
        var overwriteDictionary = new Dictionary<string, string>();
        foreach (var keyword in overwrites)
        {
            if (string.IsNullOrEmpty(keyword.UniqueName) || 
                string.IsNullOrWhiteSpace(keyword.UniqueName) ||
                string.IsNullOrEmpty(keyword.DialogueKeyword) ||
                string.IsNullOrWhiteSpace(keyword.DialogueKeyword))
            {
                throw new Exception("character name and keyword must exist for" +
                    " overwrite designation");
            }
            var name = keyword.UniqueName.ToLowerInvariant();
            if (!charDictionary.TryGetValue(name, out Tuple<string, Sprite> value))
            {
                throw new Exception("the character key to overwrite for dialogue" +
                    " was not found");
            }
            overwriteDictionary.Add(keyword.DialogueKeyword, value.Item1);
        }
        this.dialogueKeywordToDisplayName =
            new ReadOnlyDictionary<string, string>(overwriteDictionary);
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
        // character info dictionary is only temporarily required
        // also creates the dialogue dictionary as it must come immediately
        // after the other one
        var charInfoDictionary = CreateCharDictionaries(characterList);
        CreateBGDictionary(backgroundList);
        VerifyNoOverlappingCommands(CharacterDictionary, backgroundDictionary);
        var dController = dialoguePrefab.GetComponent<DialogueController>();
        dialogueCharacterLimit = dController.CharacterMax;
        ParseEventScripts(eventTextFiles, charInfoDictionary);
    }

    private static void VerifyNoOverlappingCommands(
        ReadOnlyDictionary<string, Tuple<string, Sprite>> characterDictionary, 
        ReadOnlyDictionary<string, BackgroundInfo> backgroundDictionary)
    {
        HashSet<string> checkHash = new HashSet<string>
        {
            // obviously cannot be equivalent to the 
            // comment or dialogue keyword chars either
            COMMENT.ToString(),
            DIALOGUE.ToString()
        };
        VerifyNoOverlapLoop(characterDictionary, checkHash);
        VerifyNoOverlapLoop(backgroundDictionary, checkHash);
        VerifyNoOverlapLoop(new ReadOnlyDictionary
            <string, BaseKeyword>(BASE_KEYWORDS), checkHash);
    }

    private static void VerifyNoOverlapLoop<T>(
        ReadOnlyDictionary<string, T> backgroundDictionary, 
        HashSet<string> checkHash)
    {
        foreach (var command in backgroundDictionary)
        {
            if (checkHash.Contains(command.Key))
            {
                throw new Exception("unique character names and backgrounds " +
                    "must not overlap with each other, themselves or script command words" +
                    ": " + command.Key);
            }
            checkHash.Add(command.Key);
        }
    }

    private void CreateBGDictionary(List<BackgroundInfo> bgList)
    {
        var bgDictionary = new Dictionary<string, BackgroundInfo>();
        foreach (var bgInfo in bgList)
        {
            if (bgDictionary.ContainsKey(bgInfo.UniqueName))
            {
                throw new Exception("backgrounds must have unique names: " +
                    bgInfo.UniqueName);
            }
            bgDictionary.Add(bgInfo.UniqueName, bgInfo);
        }
        this.backgroundDictionary = new ReadOnlyDictionary<string, BackgroundInfo>
            (bgDictionary);
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (useTestMode)
        {
            var emailRef = FindObjectOfType<EmailController>();
            SetEmailReference(emailRef);
            emailRef.ShowYourself();
            foreach (var runnableEvent in eventDictionary.Values)
            {
                eventController.RunEvent(runnableEvent);
            }
        }
#endif
        // playerdata.player better exist by now
        eventController.ConnectPlayerLocationListener();
        ConnectEvents(eventDictionary);
        
        
    }


    // personal note: events need to be ordered when multiple events are triggered at once
    // s.t. the closest to the previous value of the data property are done first
    // and the furthest from previous value are done last
    private void ConnectEvents(ReadOnlyDictionary<string, EventInfo> eventDictionary)
    {
        if (PlayerData.Player == null)
        {
            Debug.LogError("Player data must be instantiated for event listening");
            return;
        }
        
        foreach (var @event in eventDictionary.Values)
        {
            if (@event.EventListening)
            {
                foreach (var trigger in @event.TriggeringConditions)
                {
                    if (listeningEvents.TryGetValue(
                        trigger.Item1, out SortedList<int, List<EventInfo>> list))
                    {
                        if (list.TryGetValue(trigger.Item2, out List<EventInfo> innerList)) 
                        {
                            innerList.Add(@event);
                        }
                        else
                        {
                            list.Add(trigger.Item2, new List<EventInfo>() { @event });
                        }
                        
                    }
                    else
                    {
                        list = new SortedList<int, List<EventInfo>>
                            { { trigger.Item2, new List<EventInfo>(){@event } } };
                        listeningEvents.Add(trigger.Item1, list);
                    }
                }  
            }
        }
        UnityEventTools.AddPersistentListener(
            PlayerData.Player.OnMoneyChanged, MoneyListener);
        UnityEventTools.AddPersistentListener(
            PlayerData.Player.OnMoralityChanged, MoralityListener);
        UnityEventTools.AddPersistentListener(
            PlayerData.Player.OnShopLvlChanged, ShopLvlListener);
        UnityEventTools.AddPersistentListener(
            PlayerData.Player.OnWeekChanged, WeekListener);
    }

   

    private void MoneyListener(int previous, int current)
    {
        var keyword = TriggerKeyword.Money;
        GenericListener(previous, current, keyword);
    }

    private void ShopLvlListener(int previous, int current)
    {
        var keyword = TriggerKeyword.UpgradeLevel;
        GenericListener(previous, current, keyword);
    }

    private void MoralityListener(int previous, int current)
    {
        var keyword = TriggerKeyword.Morality;
        GenericListener(previous, current, keyword);
    }

    private void WeekListener(int previous, int current)
    {
        var keyword = TriggerKeyword.Week;
        GenericListener(previous, current, keyword);
    }

    private void GenericListener(int previous, int current, TriggerKeyword keyword)
    {
        if (listeningEvents.TryGetValue(
            keyword, out SortedList<int, List<EventInfo>> outerList))
        {
            if (current == previous) return;
            List<KeyValuePair<int, List<EventInfo>>> eventsToRun =
                FilterEventList(outerList, current);
            // need to filter based on the other triggering conditions IF
            // this is all conditions met event
            foreach (var eventList in eventsToRun)
            {
                foreach (var eventInfo in eventList.Value)
                {
                    if (ConfirmTriggered(eventInfo))
                    {
                        eventController.RunEvent(eventInfo);
                    }
                }

            }
        }
    }

    // confirms triggering after checking that one 
    // triggering condition has been met
    private bool ConfirmTriggered(EventInfo eventInfo)
    {
        // since the base trigger already got confirmed
        if (!eventInfo.AllTriggersNeeded)
        {
            return true;
        }
        foreach (var condition in eventInfo.TriggeringConditions)
        {
            int actualAmount;
            // checking for failing, returning true if no fails
            switch (condition.Item1)
            {
                case TriggerKeyword.Money:
                    actualAmount = PlayerData.Player.money;
                    break;
                case TriggerKeyword.Morality:
                    actualAmount = PlayerData.Player.morality;
                    break;
                case TriggerKeyword.UpgradeLevel:
                    actualAmount = PlayerData.Player.shoplvl;
                    break;
                case TriggerKeyword.Week:
                    actualAmount = PlayerData.Player.week;
                    break;
                default:
                    continue;
            }
            if (IsTriggerFailed(condition.Item2, actualAmount))
                return false;
        }
        return true;
    }

    private bool IsTriggerFailed(int conditionAmount, int actualAmount)
    {
        if (conditionAmount >= 0)
        {
            if (conditionAmount <= actualAmount)
                return false;
        }
        else if (conditionAmount >= actualAmount)
        {
            return false;
        }
        return true;
    }

    private static List<KeyValuePair<int, List<EventInfo>>> FilterEventList(
        SortedList<int, List<EventInfo>> list, 
        int currentAmount, int previousOrZero = 0)
    {
        List<KeyValuePair<int, List<EventInfo>>> eventsToRun;
        if (currentAmount >= previousOrZero)
        {
            // we're increasing, sort in ascending order
            // get anything greater than 0 and 
            // less than or equal to amount
            eventsToRun = list.Where(n =>
                n.Key >= previousOrZero && n.Key <= currentAmount).ToList();
            eventsToRun.Sort(new Comparison<KeyValuePair<int, List<EventInfo>>>
                ((x, y) => x.Key - y.Key));
        }
        else
        {
            // need to get events from amount to 0
            // sorts in opposite direction
            eventsToRun = list.Where(n =>
                n.Key >= currentAmount && n.Key <= previousOrZero).ToList();
            eventsToRun.Sort(new Comparison<KeyValuePair<int, List<EventInfo>>>
                ((x, y) => y.Key - x.Key));
        }
        return eventsToRun;
    }
}
