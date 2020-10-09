using General_Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [HideInInspector]
    public delegate void OnComplete();
    [HideInInspector]
    public delegate void OnChoiceComplete(int chosenValue);

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
    private List<CharacterInfo> characterList = new List<CharacterInfo>();
    private Dictionary<string, Tuple<string, Sprite>> characterDictionary =
        new Dictionary<string, Tuple<string, Sprite>>();
    private Dictionary<string, CharacterInfo> charInfoDictionary = 
        new Dictionary<string, CharacterInfo>();
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
            CharacterInfo lastCharacterDesignated = null;
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
                if (trimmedText.First()[0] == ScriptConstants.DIALOGUE)
                {
                    // only unset this when we hit the choice_end or branch_start keywords
                    if (insideChoice)
                    {
                        // need to go down the prio list --> if prompt filled go to last 
                        // choice and replace the string there
                        // if that string is not "" throw an error cuz choices only get 
                        // one line of text.
                        //throw new NotImplementedException();
                        eInfo.GetLastChoice().choicePrompt = ExtractDialogue(trimmedText);
                    }
                    else
                    {
                        RegisterDialogue(trimmedText, lastCharacterDesignated,
                            eInfo.GetLastDialogue());
                    }
                    continue;
                }

                var keywordMatch = trimmedText.First();
                if (ScriptConstants.BASE_KEYWORDS.TryGetValue(
                    keywordMatch, out ScriptConstants.BaseKeyword keyword))
                {
                    ResolveKeyword(keyword, trimmedText, ref eInfo,
                        ref lastCharacterDesignated, ref insideChoice, 
                        ref insideBranchDeclaration);
                }
                else if (charInfoDictionary.TryGetValue(keywordMatch, 
                    out lastCharacterDesignated))
                {
                    if (insideChoice)
                    {
                        eInfo.GetLastChoice().charImage = lastCharacterDesignated.Sprite;
                        eInfo.GetLastChoice().charName = lastCharacterDesignated.DisplayName;
                    }
                    continue;
                }
                else
                {
                    Debug.Log(string.Join(" ", charInfoDictionary.Keys));
                    throw new Exception(keywordMatch + " did not match any base " +
                        "keywords, comment, dialogue " +
                        "or any known character unique names. Is this dialogue/ a " +
                        "choice and missing the keyword " +
                        ScriptConstants.DIALOGUE.ToString() + " ?");
                }
            }
        }

    }

    private void ResolveKeyword(ScriptConstants.BaseKeyword value, List<string> trimmedText, 
        ref EventInfo eInfo, ref CharacterInfo lastCharacterDesignated, 
        ref bool insideChoice, ref bool insideBranchDeclaration)
    {
        DialogueInfo dInfo;
        switch (value)
        {
            // required, denotes the start of an event and its UNIQUE name
            case ScriptConstants.BaseKeyword.Event:
                if (trimmedText.Count < 2)
                {
                    throw new Exception("event needs a name on declaration line");
                }
                eInfo = new EventInfo(trimmedText[1].ToLowerInvariant().Trim());
                Debug.Log(eInfo.UniqueName);
                dInfo = new DialogueInfo(eInfo.BranchID.ToString(),
                    () => { }, characterDictionary);
                // adding a default dialogue to event info for first dialogue
                eInfo.AddDialogue(dInfo);
                eInfo.branchingDictionary.Add(EventInfo.FIRST_BRANCH,
                    new Tuple<bool, int>(true, 0));
                break;
            // not required, just allows a branch to escape an event asap
            // makes the most recent branch point to "end" as next
            case ScriptConstants.BaseKeyword.EventEarlyEnd:
                var earlyEnds = eInfo.GetLastChoice().choiceHasEarlyEnd;
                earlyEnds[earlyEnds.Count - 1] = true;
                break;
            // required, denotes the end of an event
            case ScriptConstants.BaseKeyword.EventEnd:
                if (eventDictionary.ContainsKey(eInfo.UniqueName))
                {
                    throw new Exception("cannot add duplicate event name to event " +
                        "dictionary: " + eInfo.UniqueName);
                }
                eventDictionary.Add(eInfo.UniqueName, eInfo);
                eInfo.PrintInformation();
                eInfo = null;
                break;
            case ScriptConstants.BaseKeyword.Choice:
                insideChoice = true;
                eInfo.AddChoice(new ChoiceInfo(eInfo.BranchID.ToString()));
                break;
            // required after branch declaration of 
            case ScriptConstants.BaseKeyword.ChoiceEnd:
                ChoiceDeclarationComplete(ref insideChoice, eInfo);
                break;
            // declares a branch in a choice, different from branch start which is the start
            // of a branch's specific dialogue
            case ScriptConstants.BaseKeyword.Branch:
                ChoiceInfo infoToModify = eInfo.GetLastChoice();
                infoToModify.choices.Add("");
                infoToModify.choiceHasEarlyEnd.Add(false);
                infoToModify.rewards.Add(new List<Tuple<ScriptConstants.RewardKeyword, int>>());
                insideBranchDeclaration = true;
                break;
            case ScriptConstants.BaseKeyword.BranchStart:
                insideBranchDeclaration = false;
                if (insideChoice)
                {
                    ChoiceDeclarationComplete(ref insideChoice, eInfo);
                }
                eInfo.AddDialogue(new DialogueInfo(eInfo.BranchID.ToString(),
                    () => { }, characterDictionary));
                break;
            // end of a branch's dialogue not the end of its declaration
            case ScriptConstants.BaseKeyword.BranchEnd:
                break;
            // the reward for choosing a specific choice
            case ScriptConstants.BaseKeyword.Reward:
                break;
            case ScriptConstants.BaseKeyword.Trigger:
                break;
            // immediately adds reward on branch being played
            case ScriptConstants.BaseKeyword.EventReward:
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
        if (trimmedText.First()[0] == ScriptConstants.COMMENT)
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

    private void RegisterDialogue(List<string> trimmedText,
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
        {
            trimmedText[0].Substring(1);
        }
        else
        {
            trimmedText.PopFront();
        }
        var dialogueLine = string.Join(" ", trimmedText.ToArray());
        return dialogueLine;
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

    
    void Update()
    {
        // need to check for triggers when morality / days / money changes NOT
        // in update loop ---> should be entirely unnecessary
        // so obv use c#'s event
    }
}
