using General_Utilities;
using System;
using System.Collections.Generic;

public static class Parsing_Utilities
{
    public static char COMMENT = '#';
    public static char DIALOGUE = '>';

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
        // how this event is triggered
        Trigger,
        SingleTriggerCondition,
        AllTriggerConditions,
        Reward,
        EventReward,
        BackgroundChange,
        // e.g. a quest will directly trigger it and does not need to 
        // be registered
        DirectTrigger,
    }

    public enum TriggerKeyword
    {
        Money,
        UpgradeLevel,
        Week,
        Morality,

        // will place the game over canvas underneath the dialogue of 
        // the event
        GameOver,
    }

    public enum RewardKeyword
    {
        Money,
        Morality,
        Week,
        ShopLevel,
    }

    // no uppercase letters in any of the keywords allowed
    public static readonly Dictionary<string, BaseKeyword> BASE_KEYWORDS =
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
        { "event_end_early", BaseKeyword.EventEarlyEnd },
        { "events_early_end", BaseKeyword.EventEarlyEnd },
        { "event_ends_early", BaseKeyword.EventEarlyEnd },
        { "events_end", BaseKeyword.EventEnd },
        { "choice_end", BaseKeyword.ChoiceEnd },
        { "choices_end", BaseKeyword.ChoiceEnd },
        { "branch_end", BaseKeyword.BranchEnd },
        { "branches_end", BaseKeyword.BranchEnd },
        { "branch_start", BaseKeyword.BranchStart },
        { "branches_start", BaseKeyword.BranchStart },
        { "trigger", BaseKeyword.Trigger },
        { "triggers", BaseKeyword.Trigger },
        { "direct_trigger", BaseKeyword.DirectTrigger },
        { "direct_triggers", BaseKeyword.DirectTrigger },
        { "directly_triggers", BaseKeyword.DirectTrigger },
        { "any_triggers", BaseKeyword.SingleTriggerCondition },
        { "any_trigger", BaseKeyword.SingleTriggerCondition },
        { "one_trigger", BaseKeyword.SingleTriggerCondition },
        { "one_triggers", BaseKeyword.SingleTriggerCondition },
        { "all_triggers", BaseKeyword.AllTriggerConditions },
        { "all_trigger", BaseKeyword.AllTriggerConditions },
        { "reward", BaseKeyword.Reward },
        { "event_reward", BaseKeyword.EventReward },
        { "event_rewards", BaseKeyword.EventReward },
        { "background", BaseKeyword.BackgroundChange },
        { "bg", BaseKeyword.BackgroundChange },
        { "change_bg", BaseKeyword.BackgroundChange },
        { "change_background", BaseKeyword.BackgroundChange },
    };

    // reads second word after trigger to figure out what to do next
    // if trigger trigger then event doesnt listen for anything and is called 
    // by other events only
    public static readonly Dictionary<string, TriggerKeyword> TRIGGER_KEYWORDS =
        new Dictionary<string, TriggerKeyword>
    {
        { "money" , TriggerKeyword.Money },
        { "weeks" , TriggerKeyword.Week },
        { "week" , TriggerKeyword.Week },
        { "morality" , TriggerKeyword.Morality },
        { "shop_level" , TriggerKeyword.UpgradeLevel },
        { "shop_lvl" , TriggerKeyword.UpgradeLevel },
        { "upgrade_level" , TriggerKeyword.UpgradeLevel },
        { "upgrade_lvl" , TriggerKeyword.UpgradeLevel },
        { "level" , TriggerKeyword.UpgradeLevel },
        { "game_over", TriggerKeyword.GameOver },
        { "games_over", TriggerKeyword.GameOver },
    };

    // reads second word after reward
    public static readonly Dictionary<string, RewardKeyword> REWARD_KEYWORDS =
        new Dictionary<string, RewardKeyword>
    {
        { "morality" , RewardKeyword.Morality },
        { "money" , RewardKeyword.Money },
        { "week", RewardKeyword.Week },
        { "shop_level" , RewardKeyword.ShopLevel },
        { "shop_lvl" , RewardKeyword.ShopLevel },
        { "upgrade_level" , RewardKeyword.ShopLevel },
        { "upgrade_lvl" , RewardKeyword.ShopLevel },
    };

    public delegate void ActionRef<T1>(ref T1 arg1);

    // the choice bools (tuple bool) should be named (insideChoice, insideChoiceDialogueBranch)
    public static readonly Dictionary<BaseKeyword, ActionRef<EventParsingInfo>> KeywordActions =
        new Dictionary<BaseKeyword, ActionRef<EventParsingInfo>>
        {
            {BaseKeyword.BackgroundChange, new ActionRef<EventParsingInfo>(BackgroundChangeAction) },
            {BaseKeyword.Branch, new ActionRef<EventParsingInfo>(BranchAction) },
            {BaseKeyword.BranchEnd, new ActionRef<EventParsingInfo>(BranchEndAction) },
            {BaseKeyword.BranchStart, new ActionRef<EventParsingInfo>(BranchStartAction) },
            {BaseKeyword.Choice, new ActionRef<EventParsingInfo>(ChoiceAction) },
            {BaseKeyword.ChoiceEnd, new ActionRef<EventParsingInfo>(ChoiceEndAction) },
            {BaseKeyword.DirectTrigger, new ActionRef<EventParsingInfo>(DirectTriggerAction) },
            {BaseKeyword.Event, new ActionRef<EventParsingInfo>(EventAction) },
            {BaseKeyword.EventEarlyEnd, new ActionRef<EventParsingInfo>(EventEarlyEndAction) },
            {BaseKeyword.EventEnd, new ActionRef<EventParsingInfo>(EventEndAction) },
            {BaseKeyword.EventReward, new ActionRef<EventParsingInfo>(EventRewardAction) },
            {BaseKeyword.Reward, new ActionRef<EventParsingInfo>(RewardAction) },
            {BaseKeyword.Trigger, new ActionRef<EventParsingInfo>(TriggerAction) },
            {BaseKeyword.SingleTriggerCondition, new ActionRef<EventParsingInfo>(SingleTriggerAction) },
            {BaseKeyword.AllTriggerConditions, new ActionRef<EventParsingInfo>(AllTriggersAction) },
        };

    public static readonly Dictionary<TriggerKeyword, ActionRef<EventParsingInfo>> TriggerKeywordActions =
        new Dictionary<TriggerKeyword, ActionRef<EventParsingInfo>>
        {
            {TriggerKeyword.Money, new ActionRef<EventParsingInfo>(MoneyTriggerAction) },
            {TriggerKeyword.Morality, new ActionRef<EventParsingInfo>(MoralityTriggerAction) },
            {TriggerKeyword.UpgradeLevel, new ActionRef<EventParsingInfo>(UpgradeLevelTriggerAction) },
            {TriggerKeyword.Week, new ActionRef<EventParsingInfo>(WeekTriggerAction) },
        };

    private static void WeekTriggerAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.TrimmedLine.Count < 3)
        {
            throw new Exception("triggers must have name of an amount specified");
        }
        var amt = int.Parse(parsingInfo.TrimmedLine[2].ToLowerInvariant().Trim());
        parsingInfo.EventInfo.TriggeringConditions.Add(
            new Tuple<TriggerKeyword, int>(TriggerKeyword.Week, amt));
    }

    private static void UpgradeLevelTriggerAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.TrimmedLine.Count < 3)
        {
            throw new Exception("triggers must have name of an amount specified");
        }
        var amt = int.Parse(parsingInfo.TrimmedLine[2].ToLowerInvariant().Trim());
        parsingInfo.EventInfo.TriggeringConditions.Add(
            new Tuple<TriggerKeyword, int>(TriggerKeyword.UpgradeLevel, amt));
    }

    private static void MoralityTriggerAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.TrimmedLine.Count < 3)
        {
            throw new Exception("triggers must have name of an amount specified");
        }
        var amt = int.Parse(parsingInfo.TrimmedLine[2].ToLowerInvariant().Trim());
        parsingInfo.EventInfo.TriggeringConditions.Add(
            new Tuple<TriggerKeyword, int>(TriggerKeyword.Morality, amt));
    }

    private static void MoneyTriggerAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.TrimmedLine.Count < 3)
        {
            throw new Exception("triggers must have name of an amount specified");
        }
        var amt = int.Parse(parsingInfo.TrimmedLine[2].ToLowerInvariant().Trim());
        parsingInfo.EventInfo.TriggeringConditions.Add(
            new Tuple<TriggerKeyword, int>(TriggerKeyword.Money, amt));
    }

    private static void DirectTriggerAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.TrimmedLine.Count < 2)
        {
            throw new Exception("direct triggers must have name of event");
        }
        var eventName = parsingInfo.TrimmedLine[1].ToLowerInvariant().Trim();
        if (parsingInfo.IsChoiceIsChoiceDialogue.Item1)
        {
            parsingInfo.EventInfo.MultiEventTriggerWrite(
                parsingInfo.ChoiceDialoguesToMultiWrite, eventName);
        }
        else
        {
            parsingInfo.EventInfo.GetLastDialogue().DirectlyTriggeredEvents.Add(eventName);
        }
    }

    // item1 of choice bools is whether in choice, item2 is whether in a choice branch's dialogue
    private static void BackgroundChangeAction(ref EventParsingInfo parsingInfo)
    {
        UnityEngine.Debug.LogError("background changing not implemented yet");
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
        parsingInfo.EventInfo = new EventInfo(parsingInfo.TrimmedLine[1].ToLowerInvariant().Trim());
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

    private static void EventRewardAction(ref EventParsingInfo parsingInfo)
    {
        if (parsingInfo.IsChoiceIsChoiceDialogue.Item1)
        {
            throw new Exception("event complete rewards may not be declared inside a choice");
        }
        parsingInfo.EventInfo.EventCompleteReward.Add(
            GetRewardTuple(parsingInfo.TrimmedLine));
    }

    private static void RewardAction(ref EventParsingInfo parsingInfo)
    {
        if (!parsingInfo.IsChoiceIsChoiceDialogue.Item1)
        {
            parsingInfo.EventInfo.PrintInformation();
            throw new Exception("can only provide normal rewards inside a choice" +
                " declaration");
        }
        var reward = GetRewardTuple(parsingInfo.TrimmedLine);
        parsingInfo.EventInfo.GetLastChoice().AddReward(reward.Item1, reward.Item2);
    }



    private static void TriggerAction(ref EventParsingInfo parsingInfo)
    {
        var triggerKeyword = parsingInfo.TrimmedLine[1].ToLowerInvariant().Trim();
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

    private static Tuple<RewardKeyword, int> GetRewardTuple(List<string> line)
    {
        if (line.Count < 3)
        {
            throw new Exception("reward command must specify the reward type and amount");
        }
        var rewardKey = line[1].ToLowerInvariant().Trim();
        if (REWARD_KEYWORDS.TryGetValue(rewardKey, out RewardKeyword rewardType))
        {
            // failure is desired if it doesnt work
            var rewardAmount = int.Parse(line[2].ToLowerInvariant().Trim());
            return new Tuple<RewardKeyword, int>(rewardType, rewardAmount);
        }
        throw new Exception(
            "reward key given is not in reward dictionary: " + rewardKey);
    }
}
