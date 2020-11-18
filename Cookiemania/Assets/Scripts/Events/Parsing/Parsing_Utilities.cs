using System.Collections.Generic;

public static partial class Parsing_Utilities
{
    public static readonly Dictionary<BaseKeyword, ActionRef<EventParsingInfo>> KeywordActions =
        new Dictionary<BaseKeyword, ActionRef<EventParsingInfo>>
        {
            { BaseKeyword.Stage, new ActionRef<EventParsingInfo>(SetStageAction) },
            { BaseKeyword.Type, new ActionRef<EventParsingInfo>(TypeAction) },
            { BaseKeyword.Delay, new ActionRef<EventParsingInfo>(DelayAction) },
            { BaseKeyword.Subject, new ActionRef<EventParsingInfo>(SubjectAction) },
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
            { TypeKeyword.EventEmail, new ActionRef<EventParsingInfo>(EmailTypeAction) },
            { TypeKeyword.TutorialEmail, new ActionRef<EventParsingInfo>(TutorialEmailTypeAction) },
            { TypeKeyword.HistoryEmail, new ActionRef<EventParsingInfo>(HistoryEmailTypeAction) },
            { TypeKeyword.Reward, new ActionRef<EventParsingInfo>(RewardTypeAction) },
            { TypeKeyword.Tutorial, new ActionRef<EventParsingInfo>(TutorialTypeAction) },
        };

    public static readonly Dictionary<Locale, ActionRef<EventParsingInfo>> TutorialKeywordActions =
       new Dictionary<Locale, ActionRef<EventParsingInfo>>
       {
           { Locale.AnalyticsTab, new ActionRef<EventParsingInfo>(AnalyticsDelayAction) },
           { Locale.Desktop, new ActionRef<EventParsingInfo>(DesktopDelayAction) },
           { Locale.EmailTab, new ActionRef<EventParsingInfo>(EmailDelayAction) },
           { Locale.JumpingMinigame, new ActionRef<EventParsingInfo>(JumperDelayAction) },
           { Locale.Minigame, new ActionRef<EventParsingInfo>(MinigameDelayAction) },
           { Locale.Any, new ActionRef<EventParsingInfo>(NoDelayAction) },
           { Locale.SpaceMinigame, new ActionRef<EventParsingInfo>(SpaceMinigameDelayAction) },
           { Locale.WebsiteTab, new ActionRef<EventParsingInfo>(WebsiteDelayAction) },
       };
}
