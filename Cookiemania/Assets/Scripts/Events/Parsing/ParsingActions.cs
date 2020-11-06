using System;
using General_Utilities;

public static partial class Parsing_Utilities
{

    public delegate void ActionRef<T1>(ref T1 arg1);

    public static void GenericTutorialAction(ref EventParsingInfo parsingInfo,
    TutorialKeyword type)
    {

    }

    public static void WebsiteTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    public static void SpaceMinigameTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    public static void NoneTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    public static void MinigameTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    public static void JumperTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    public static void EmailTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    public static void DesktopTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    public static void AnalyticsTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    public static void RewardTypeAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.EventType = TypeKeyword.Reward;
    }

    public static void TutorialTypeAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.EventType = TypeKeyword.Tutorial;
        // need to also get the tutorial start location
    }

    public static void EmailTypeAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.EventType = TypeKeyword.Email;
    }

    public static void DialogueTypeAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.EventType = TypeKeyword.Dialogue;
    }

    public static void GenericTriggerAction(ref EventParsingInfo parsingInfo, TriggerKeyword type)
    {
        if (parsingInfo.TrimmedLine.Count < 3)
        {
            throw new Exception("triggers must have name of an amount specified");
        }
        var amt = parsingInfo.GetParsedInt(2);
        parsingInfo.EventInfo.TriggeringConditions.Add(
            new Tuple<TriggerKeyword, int>(type, amt));
    }

    public static void WeekTriggerAction(ref EventParsingInfo parsingInfo)
    {
        GenericTriggerAction(ref parsingInfo, TriggerKeyword.Week);
    }

    public static void UpgradeLevelTriggerAction(ref EventParsingInfo parsingInfo)
    {
        GenericTriggerAction(ref parsingInfo, TriggerKeyword.UpgradeLevel);
    }

    public static void MoralityTriggerAction(ref EventParsingInfo parsingInfo)
    {
        GenericTriggerAction(ref parsingInfo, TriggerKeyword.Morality);
    }

    public static void MoneyTriggerAction(ref EventParsingInfo parsingInfo)
    {
        GenericTriggerAction(ref parsingInfo, TriggerKeyword.Money);
    }

    public static void DirectTriggerAction(ref EventParsingInfo parsingInfo)
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

    public static void SetStageAction(ref EventParsingInfo parsingInfo)
    {
        UnityEngine.Debug.LogError("dynamic background changing not implemented yet");
        return;
    }

    public static void AllTriggersAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.AllTriggersNeeded = true;
    }

    public static void SingleTriggerAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.AllTriggersNeeded = false;
    }

    public static void BranchAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.IsChoiceIsChoiceDialogue = new Tuple<bool, bool>(true, false);
        parsingInfo.EventInfo.GetLastChoice().AddChoice("");
    }

    // empty as currently intended
    public static void BranchEndAction(ref EventParsingInfo parsingInfo)
    {
        return;
    }

    public static void BranchStartAction(ref EventParsingInfo parsingInfo)
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

    public static void ChoiceAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.IsChoiceIsChoiceDialogue = new Tuple<bool, bool>(true, false);
        parsingInfo.EventInfo.AddChoice(new ChoiceInfo(
            parsingInfo.EventInfo.BranchID.ToString(), parsingInfo.MaxChoices));
        // need to add this choice's branch name to whatever the previous dialogue branch was
        parsingInfo.EventInfo.SetNextBranch(parsingInfo.EventInfo.GetLastDialogue().UniqueName,
            parsingInfo.EventInfo.GetLastChoice().UniqueName);
    }

    public static void ChoiceEndAction(ref EventParsingInfo parsingInfo)
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

    public static void EventAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.TrimmedLine.Count < 2)
        {
            throw new Exception("event needs a name on declaration line");
        }
        parsingInfo.EventInfo = new EventInfo(parsingInfo.GetLowercaseWord(1));
    }

    public static void EventEarlyEndAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.IsChoiceIsChoiceDialogue.Item1)
            parsingInfo.EventInfo.MultiEarlyExitWrite(parsingInfo.ChoiceDialoguesToMultiWrite);
        else
            parsingInfo.EventInfo.GetLastDialogue().ExitsEvent = true;
        BranchEndAction(ref parsingInfo);
    }

    public static void EventEndAction(ref EventParsingInfo parsingInfo)
    {
        parsingInfo.EventInfo.GetLastDialogue().NextBranch = EventInfo.LAST_BRANCH;
        parsingInfo.EventInfo.GetLastDialogue().ExitsEvent = true;
        if (parsingInfo.EventInfos.ContainsKey(parsingInfo.EventInfo.UniqueName))
        {
            throw new Exception("cannot add duplicate event name to event " +
                "dictionary: " + parsingInfo.EventInfo.UniqueName);
        }
        parsingInfo.EventInfos.Add(parsingInfo.EventInfo.UniqueName,
            parsingInfo.EventInfo);
        parsingInfo.ResetForNextEvent();
    }

    public static void EventRewardAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.IsChoiceIsChoiceDialogue.Item1)
        {
            throw new Exception("event complete rewards may not be declared inside a choice");
        }
        parsingInfo.EventInfo.EventCompleteReward.Add(
            GetRewardTuple(parsingInfo));
    }

    public static void RewardAction(ref EventParsingInfo parsingInfo)
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

    public static void TriggerAction(ref EventParsingInfo parsingInfo)
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

    public static void ChoiceDeclarationComplete(EventInfo eventInfo)
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

    public static Tuple<RewardKeyword, int> GetRewardTuple(EventParsingInfo parsingInfo)
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
