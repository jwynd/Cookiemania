using System.Collections.Generic;

public static partial class Parsing_Utilities
{
    public static char COMMENT = '#';
    public static char DIALOGUE = '>';

    public enum BaseKeyword
    {
        Event,
        Type,
        // delay the event to be run after it is triggered
        // until the player is in the appropriate screen
        // defined by the DelayedRunKeyword enums
        Delay,
        // email only
        Subject,
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
        // just the normal dialogue event and NOT
        // a tutorial
        Dialogue,
        Reward,
        // game tutorials, has specific triggering action e.g.
        // specific minigame is opened or specific thing is 
        // bought in the shop ?
        Tutorial,
        EventEmail,
        TutorialEmail,
        HistoryEmail,
    }

    public static bool IsEmail(this TypeKeyword type)
    {
        return (
            type == TypeKeyword.EventEmail ||
            type == TypeKeyword.HistoryEmail ||
            type == TypeKeyword.TutorialEmail);
    }

    public enum DelayedRunKeyword
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
        { "delay", BaseKeyword.Delay },
        { "delayed_until", BaseKeyword.Delay },
        { "delay_until", BaseKeyword.Delay },
        { "subject", BaseKeyword.Subject },
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
            { "email", TypeKeyword.EventEmail },
            { "email_tutorial", TypeKeyword.TutorialEmail },
            { "tutorial", TypeKeyword.Tutorial },
            { "email_history", TypeKeyword.HistoryEmail },
            { "reward", TypeKeyword.Reward },
            { "none", TypeKeyword.Reward },
        };

    public static readonly Dictionary<string, DelayedRunKeyword> DELAY_KEYWORDS =
        new Dictionary<string, DelayedRunKeyword>
        {
            { "default", DelayedRunKeyword.None },
            { "none", DelayedRunKeyword.None },
            { "any", DelayedRunKeyword.None },
            { "analytics", DelayedRunKeyword.AnalyticsTab },
            { "analytics_tab", DelayedRunKeyword.AnalyticsTab },
            { "desktop", DelayedRunKeyword.Desktop },
            { "email", DelayedRunKeyword.EmailTab },
            { "email_tab", DelayedRunKeyword.EmailTab },
            { "jumper_minigame", DelayedRunKeyword.JumpingMinigame },
            { "jumping_minigame", DelayedRunKeyword.JumpingMinigame },
            { "jumper", DelayedRunKeyword.JumpingMinigame },
            { "minigame", DelayedRunKeyword.Minigame },
            { "space_minigame", DelayedRunKeyword.SpaceMinigame },
            { "website", DelayedRunKeyword.WebsiteTab },
            { "website_tab", DelayedRunKeyword.WebsiteTab },
        };
}
