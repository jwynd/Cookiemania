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
        Type,
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
        Stage,
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

    public enum TypeKeyword
    {
        Email,
        // just the normal dialogue event and NOT
        // a tutorial
        Dialogue,
        Reward,
        // game tutorials, has specific triggering action e.g.
        // specific minigame is opened or specific thing is 
        // bought in the shop ?
        Tutorial,
    }

    // what must be on screen for the tutorial to next pop up
    // can queue up multiple tutorials
    public enum TutorialKeyword
    {
        // none means it will immediately display
        None,
        // any minigame
        Minigame,
        SpaceMinigame,
        JumpingMinigame,
        // any desktop tab
        Desktop,
        EmailTab,
        AnalyticsTab,
        WebsiteTab,
    }

    // no uppercase letters in any of the keywords allowed
    public static readonly Dictionary<string, BaseKeyword> BASE_KEYWORDS =
        new Dictionary<string, BaseKeyword>
    {
        { "event" , BaseKeyword.Event },
        { "events", BaseKeyword.Event },
        { "type", BaseKeyword.Type },
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
        { "stage", BaseKeyword.Stage },
        { "set_stage", BaseKeyword.Stage },
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

    public static readonly Dictionary<string, TypeKeyword> TYPE_KEYWORDS =
        new Dictionary<string, TypeKeyword>
        {
            { "default", TypeKeyword.Dialogue },
            { "dialogue", TypeKeyword.Dialogue },
            { "email", TypeKeyword.Email },
            { "reward", TypeKeyword.Reward },
            { "none", TypeKeyword.Reward },
        };

    public static readonly Dictionary<string, TutorialKeyword> TUTORIAL_KEYWORDS =
        new Dictionary<string, TutorialKeyword>
        {
            { "default", TutorialKeyword.None },
            { "none", TutorialKeyword.None },
            { "any", TutorialKeyword.None },
            { "analytics", TutorialKeyword.AnalyticsTab },
            { "analytics_tab", TutorialKeyword.AnalyticsTab },
            { "desktop", TutorialKeyword.Desktop },
            { "email", TutorialKeyword.EmailTab },
            { "email_tab", TutorialKeyword.EmailTab },
            { "jumper_minigame", TutorialKeyword.JumpingMinigame },
            { "jumping_minigame", TutorialKeyword.JumpingMinigame },
            { "jumper", TutorialKeyword.JumpingMinigame },
            { "minigame", TutorialKeyword.Minigame },
            { "space_minigame", TutorialKeyword.SpaceMinigame },
            { "website", TutorialKeyword.WebsiteTab },
            { "website_tab", TutorialKeyword.WebsiteTab },
        };

    public delegate void ActionRef<T1>(ref T1 arg1);

    // the choice bools (tuple bool) should be named (insideChoice, insideChoiceDialogueBranch)
    public static readonly Dictionary<BaseKeyword, ActionRef<EventParsingInfo>> KeywordActions =
        new Dictionary<BaseKeyword, ActionRef<EventParsingInfo>>
        {
            { BaseKeyword.Stage, new ActionRef<EventParsingInfo>(SetStageAction) },
            { BaseKeyword.Branch, new ActionRef<EventParsingInfo>(BranchAction) },
            { BaseKeyword.BranchEnd, new ActionRef<EventParsingInfo>(BranchEndAction) },
            { BaseKeyword.BranchStart, new ActionRef<EventParsingInfo>(BranchStartAction) },
            { BaseKeyword.Choice, new ActionRef<EventParsingInfo>(ChoiceAction) },
            { BaseKeyword.ChoiceEnd, new ActionRef<EventParsingInfo>(ChoiceEndAction) },
            { BaseKeyword.DirectTrigger, new ActionRef<EventParsingInfo>(DirectTriggerAction) },
            { BaseKeyword.Event, new ActionRef<EventParsingInfo>(EventAction) },
            { BaseKeyword.EventEarlyEnd, new ActionRef<EventParsingInfo>(EventEarlyEndAction) },
            { BaseKeyword.EventEnd, new ActionRef<EventParsingInfo>(EventEndAction) },
            { BaseKeyword.EventReward, new ActionRef<EventParsingInfo>(EventRewardAction) },
            { BaseKeyword.Reward, new ActionRef<EventParsingInfo>(RewardAction) },
            { BaseKeyword.Trigger, new ActionRef<EventParsingInfo>(TriggerAction) },
            { BaseKeyword.SingleTriggerCondition, new ActionRef<EventParsingInfo>(SingleTriggerAction) },
            { BaseKeyword.AllTriggerConditions, new ActionRef<EventParsingInfo>(AllTriggersAction) },
        };

    public static readonly Dictionary<TriggerKeyword, ActionRef<EventParsingInfo>> TriggerKeywordActions =
        new Dictionary<TriggerKeyword, ActionRef<EventParsingInfo>>
        {
            { TriggerKeyword.Money, new ActionRef<EventParsingInfo>(MoneyTriggerAction) },
            { TriggerKeyword.Morality, new ActionRef<EventParsingInfo>(MoralityTriggerAction) },
            { TriggerKeyword.UpgradeLevel, new ActionRef<EventParsingInfo>(UpgradeLevelTriggerAction) },
            { TriggerKeyword.Week, new ActionRef<EventParsingInfo>(WeekTriggerAction) },
        };

    public static readonly Dictionary<TypeKeyword, ActionRef<EventParsingInfo>> TypeKeywordActions =
        new Dictionary<TypeKeyword, ActionRef<EventParsingInfo>>
        {
            { TypeKeyword.Dialogue, new ActionRef<EventParsingInfo>(DialogueTypeAction) },
            { TypeKeyword.Email, new ActionRef<EventParsingInfo>(EmailTypeAction) },
            { TypeKeyword.Reward, new ActionRef<EventParsingInfo>(RewardTypeAction) },
        };

    public static readonly Dictionary<TutorialKeyword, ActionRef<EventParsingInfo>> TutorialKeywordActions =
       new Dictionary<TutorialKeyword, ActionRef<EventParsingInfo>>
       {
           { TutorialKeyword.AnalyticsTab, new ActionRef<EventParsingInfo>(AnalyticsTutorialAction) },
           { TutorialKeyword.Desktop, new ActionRef<EventParsingInfo>(DesktopTutorialAction) },
           { TutorialKeyword.EmailTab, new ActionRef<EventParsingInfo>(EmailTutorialAction) },
           { TutorialKeyword.JumpingMinigame, new ActionRef<EventParsingInfo>(JumperTutorialAction) },
           { TutorialKeyword.Minigame, new ActionRef<EventParsingInfo>(MinigameTutorialAction) },
           { TutorialKeyword.None, new ActionRef<EventParsingInfo>(NoneTutorialAction) },
           { TutorialKeyword.SpaceMinigame, new ActionRef<EventParsingInfo>(SpaceMinigameTutorialAction) },
           { TutorialKeyword.WebsiteTab, new ActionRef<EventParsingInfo>(WebsiteTutorialAction) },
       };

    private static void WebsiteTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    private static void SpaceMinigameTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    private static void NoneTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    private static void MinigameTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    private static void JumperTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    private static void EmailTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    private static void DesktopTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    private static void AnalyticsTutorialAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    private static void RewardTypeAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    private static void EmailTypeAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
    }

    private static void DialogueTypeAction(ref EventParsingInfo parsingInfo)
    {
        throw new NotImplementedException();
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
