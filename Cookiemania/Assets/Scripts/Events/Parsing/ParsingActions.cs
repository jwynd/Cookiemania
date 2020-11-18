using System;
using General_Utilities;

public static partial class Parsing_Utilities
{

    public delegate void ActionRef<T1>(ref T1 arg1);

    private static void SubjectAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.AddDialogue(new DialogueInfo(EventInfo.EMAIL_SUBJECT));
        parsingInfo.TrimmedLine.PopFront();
        if (parsingInfo.TrimmedLine.Count < 1)
        {
            throw new Exception("email subject must be immediately defined " +
                "e.g. subject Hey what's up!");
        }
        if (parsingInfo.CharacterInfo == null)
        {
            throw new Exception("the email sender must be defined before " +
                "the subject / body of the email");
        }
        // emails dont have to worry about character limits
        var line = string.Join(" ", parsingInfo.TrimmedLine.ToArray());
        parsingInfo.EventInfo.GetLastDialogue().AddDialogue(
            line, 
            parsingInfo.CharacterInfo.UniqueName);
        // for the body
        parsingInfo.EventInfo.AddDialogue(new DialogueInfo(
            EventInfo.EMAIL_BODY));
    }

    private static void DelayAction(ref EventParsingInfo parsingInfo)
    {
        Locale keyword;
        if (parsingInfo.TrimmedLine.Count < 2)
        {
            throw new Exception("delay location keyword not found in line: "
                + parsingInfo.TrimmedLine);
        }
        // need to also get the delayed start location
        else if (DELAY_KEYWORDS.TryGetValue(
            parsingInfo.GetLowercaseWord(1), out Locale value))
        {
            keyword = value;
        }
        else
        {
            throw new Exception("delay keyword not found for "
                + parsingInfo.GetLowercaseWord(2) + " in line: "
                + parsingInfo.TrimmedLine);
        }
        DefineDelayLocation(ref parsingInfo, keyword);
    }

    private static void WebsiteDelayAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.DelayOption = Locale.WebsiteTab;
    }

    private static void SpaceMinigameDelayAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.DelayOption = Locale.SpaceMinigame;
    }

    private static void NoDelayAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.DelayOption = Locale.Any;
    }

    private static void MinigameDelayAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.DelayOption = Locale.Minigame;
    }

    private static void JumperDelayAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.DelayOption = Locale.JumpingMinigame;
    }

    private static void EmailDelayAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.DelayOption = Locale.EmailTab;
    }

    private static void DesktopDelayAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.DelayOption = Locale.Desktop;
    }

    private static void AnalyticsDelayAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.DelayOption = Locale.AnalyticsTab;
    }

    private static void TutorialTypeAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.EventType = TypeKeyword.Tutorial;
       
    }

    private static void DefineDelayLocation(
        ref EventParsingInfo parsingInfo, 
        Locale keyword)
    {
        if (TutorialKeywordActions.TryGetValue(
            keyword, out ActionRef<EventParsingInfo> toRun))
        {
            toRun.Invoke(ref parsingInfo);
        }
        else
        {
            throw new Exception("no action not defined for: "
                + keyword);
        }
    }

    private static void RewardTypeAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.EventType = TypeKeyword.Reward;
    }

    private static void EmailTypeAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.EventType = TypeKeyword.EventEmail;
    }

    private static void TutorialEmailTypeAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.EventType = TypeKeyword.TutorialEmail;
        // this is a summary of a tutorial that would be triggered by a normal tutorial event
    }

    private static void HistoryEmailTypeAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.EventType = TypeKeyword.HistoryEmail;
    }

    private static void DialogueTypeAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.EventType = TypeKeyword.Dialogue;
    }

    private static void TypeAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.TrimmedLine.Count < 2)
        {
            throw new Exception("event type declaration must have at least 2 words");
        }
        var type = parsingInfo.GetLowercaseWord(1);
        if (TYPE_KEYWORDS.TryGetValue(
            type, out TypeKeyword typeKeyword))
        {
            RunTypeAction(ref parsingInfo, typeKeyword);
        }
        else
        {
            throw new Exception("event type not found for input: " + type);
        }
    }

    private static void RunTypeAction(
        ref EventParsingInfo parsingInfo, 
        TypeKeyword typeKeyword)
    {
        if (TypeKeywordActions.TryGetValue(
            typeKeyword, out ActionRef<EventParsingInfo> action))
        {
            action.Invoke(ref parsingInfo);
        }
        else
        {
            throw new Exception("action not found for type: "
                + typeKeyword);
        }
    }

    private static void GenericTriggerAction(ref EventParsingInfo parsingInfo, TriggerKeyword type)
    {
        if (parsingInfo.TrimmedLine.Count < 3)
        {
            throw new Exception("triggers must have name of an amount specified");
        }
        var amt = parsingInfo.GetParsedInt(2);
        parsingInfo.EventInfo.TriggeringConditions.Add(
            new Tuple<TriggerKeyword, int>(type, amt));
    }

    private static void WeekTriggerAction(ref EventParsingInfo parsingInfo)
    {
        GenericTriggerAction(ref parsingInfo, TriggerKeyword.Week);
    }

    private static void UpgradeLevelTriggerAction(ref EventParsingInfo parsingInfo)
    {
        GenericTriggerAction(ref parsingInfo, TriggerKeyword.UpgradeLevel);
    }

    private static void MoralityTriggerAction(ref EventParsingInfo parsingInfo)
    {
        GenericTriggerAction(ref parsingInfo, TriggerKeyword.Morality);
    }

    private static void MoneyTriggerAction(ref EventParsingInfo parsingInfo)
    {
        GenericTriggerAction(ref parsingInfo, TriggerKeyword.Money);
    }

    private static void DirectTriggerAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.TrimmedLine.Count < 2)
        {
            throw new Exception("direct triggers must have name of event");
        }
        var eventName = parsingInfo.GetLowercaseWord(1);
        if (parsingInfo.IsChoiceIsChoiceDialogue.Item1)
        {
            parsingInfo.EventInfo.MultiEventTriggerWrite(
                parsingInfo.ChoiceDialoguesToMultiWrite, eventName);
        }
        else
        {
            parsingInfo.EventInfo.GetLastDialogue().
                DirectlyTriggeredEvents.Add(eventName);
        }
    }

    private static void SetStageAction(ref EventParsingInfo parsingInfo)
    {
        UnityEngine.Debug.LogError("dynamic background changing not implemented yet");
        return;
    }

    private static void AllTriggersAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.AllTriggersNeeded = true;
    }

    private static void SingleTriggerAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.AllTriggersNeeded = false;
    }

    private static void BranchAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.IsChoiceIsChoiceDialogue = new Tuple<bool, bool>(true, false);
        parsingInfo.EventInfo.GetLastChoice().AddChoice("");
    }

    // empty as currently intended
    private static void BranchEndAction(ref EventParsingInfo parsingInfo)
    {
        return;
    }

    private static void BranchStartAction(ref EventParsingInfo parsingInfo)
    {
        if (!parsingInfo.IsChoiceIsChoiceDialogue.Item1)
        {
            throw new Exception("must be in a choice to create choice branches");
        }
        // gets flipped to true for choice declaration completion as the next
        // dialogues must be the dialogues associated with this choice
        ChoiceDeclarationComplete(parsingInfo.EventInfo);
        parsingInfo.IsChoiceIsChoiceDialogue = new Tuple<bool, bool>(true, true);
        parsingInfo.TrimmedLine.PopFront();
        parsingInfo.ChoiceDialoguesToMultiWrite.Clear();
        var choice = parsingInfo.EventInfo.GetLastChoice();
        foreach (var str in parsingInfo.TrimmedLine)
        {
            // actual index is 1 less
            var index = int.Parse(str) - 1;
            if (choice.ChoiceDialogueDictionary.TryGetValue(index, out string name))
            {
                parsingInfo.ChoiceDialoguesToMultiWrite.Add(name);
            }
            else
            {
                throw new Exception(index + " not found in choice dictionary");
            }
        }
    }

    private static void ChoiceAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.IsChoiceIsChoiceDialogue = new Tuple<bool, bool>(true, false);
        parsingInfo.EventInfo.AddChoice(new ChoiceInfo(
            parsingInfo.EventInfo.BranchID.ToString(), parsingInfo.MaxChoices));
        // need to add this choice's branch name to whatever the previous dialogue branch was
        parsingInfo.EventInfo.SetNextBranch(parsingInfo.EventInfo.GetLastDialogue().UniqueName,
            parsingInfo.EventInfo.GetLastChoice().UniqueName);
    }

    private static void ChoiceEndAction(ref EventParsingInfo parsingInfo)
    {
        BranchEndAction(ref parsingInfo);
        parsingInfo.IsChoiceIsChoiceDialogue = new Tuple<bool, bool>(false, false);
        // also create next dialogue here and point choices to that dialogue
        var choice = parsingInfo.EventInfo.GetLastChoice();
        parsingInfo.EventInfo.AddDialogue(new DialogueInfo(parsingInfo.EventInfo.BranchID.ToString()));
        var nextBranch = parsingInfo.EventInfo.GetLastDialogue().UniqueName;
        foreach (var ch in choice.ChoiceDialogueDictionary.Values)
        {
            parsingInfo.EventInfo.SetNextBranch(ch, nextBranch);
        }
    }

    private static void EventAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.TrimmedLine.Count < 2)
        {
            throw new Exception("event needs a name on declaration line");
        }
        parsingInfo.EventInfo = new EventInfo(parsingInfo.GetLowercaseWord(1));
    }

    private static void EventEarlyEndAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.IsChoiceIsChoiceDialogue.Item1)
            parsingInfo.EventInfo.MultiEarlyExitWrite(parsingInfo.ChoiceDialoguesToMultiWrite);
        else
            parsingInfo.EventInfo.GetLastDialogue().ExitsEvent = true;
        BranchEndAction(ref parsingInfo);
    }

    private static void EventEndAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.FinalizeCreation();
        if (parsingInfo.EventInfos.ContainsKey(parsingInfo.EventInfo.UniqueName))
        {
            throw new Exception("cannot add duplicate event name to event " +
                "dictionary: " + parsingInfo.EventInfo.UniqueName);
        }
        parsingInfo.EventInfos.Add(parsingInfo.EventInfo.UniqueName,
            parsingInfo.EventInfo);
        parsingInfo.ResetForNextEvent();
    }

    private static void EventRewardAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.IsChoiceIsChoiceDialogue.Item1)
        {
            throw new Exception("event complete rewards may not be declared inside a choice");
        }
        parsingInfo.EventInfo.EventCompleteReward.Add(
            GetRewardTuple(parsingInfo));
    }

    private static void RewardAction(ref EventParsingInfo parsingInfo)
    {
        if (!parsingInfo.IsChoiceIsChoiceDialogue.Item1)
        {
            parsingInfo.EventInfo.PrintInformation();
            throw new Exception("can only provide normal rewards inside a choice" +
                " declaration");
        }
        var reward = GetRewardTuple(parsingInfo);
        parsingInfo.EventInfo.GetLastChoice().AddReward(reward.Item1, reward.Item2);
    }

    private static void TriggerAction(ref EventParsingInfo parsingInfo)
    {
        var triggerKeyword = parsingInfo.GetLowercaseWord(1);
        if (TRIGGER_KEYWORDS.TryGetValue(triggerKeyword, out TriggerKeyword trigger))
        {
            if (TriggerKeywordActions.TryGetValue(trigger, out ActionRef<EventParsingInfo> action))
            {
                action.Invoke(ref parsingInfo);
            }
            else
            {
                throw new Exception("trigger keyword has no " +
                    "associated action: " + trigger.ToString());
            }
        }
        else
        {
            throw new Exception("trigger keyword not known: " + triggerKeyword);
        }
    }

    private static void ChoiceDeclarationComplete(EventInfo eventInfo)
    {
        if (!eventInfo.GetLastChoice().IsFilledOutAndCorrect())
        {
            eventInfo.GetLastChoice().PrintInformation();
            throw new Exception("above choice not correctly filled out");
        }
        var choice = eventInfo.GetLastChoice();
        if (choice.ChoiceDialogueDictionary.Keys.Count == choice.Choices.Count)
            return;
        for (var i = 0; i < choice.Choices.Count; i++)
        {
            eventInfo.AddDialogue(new DialogueInfo(eventInfo.BranchID.ToString()));
            choice.AddChoiceDialogueName(i, eventInfo.GetLastDialogue().UniqueName);
        }
    }

    private static Tuple<RewardKeyword, int> GetRewardTuple(EventParsingInfo parsingInfo)
    {
        if (parsingInfo.TrimmedLine.Count < 3)
        {
            throw new Exception("reward command must specify the reward type and amount");
        }
        var rewardKey = parsingInfo.GetLowercaseWord(1);
        if (REWARD_KEYWORDS.TryGetValue(rewardKey, out RewardKeyword rewardType))
        {
            // failure is desired if it doesnt work
            var rewardAmount = parsingInfo.GetParsedInt(2);
            return new Tuple<RewardKeyword, int>(rewardType, rewardAmount);
        }
        throw new Exception(
            "reward key given is not in reward dictionary: " + rewardKey);
    }
}
