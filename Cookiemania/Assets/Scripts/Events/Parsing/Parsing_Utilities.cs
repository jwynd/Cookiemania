using System.Collections.Generic;

public static partial class Parsing_Utilities
{
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
            { TypeKeyword.Tutorial, new ActionRef<EventParsingInfo>(TutorialTypeAction) },
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


}
