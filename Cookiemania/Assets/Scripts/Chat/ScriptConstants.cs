using System.Collections.Generic;

public class ScriptConstants
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
        Trigger,
        Reward,
        EventReward,
        BackgroundChange,
    }

    public enum TriggerKeyword
    {
        EventStart,
        EventEnd,
        Money,
        UpgradeLevel,
        Week,
        Morality,
        // e.g. a quest will directly trigger it and does not need to 
        // be registered
        DirectTrigger,
    }

    public enum RewardKeyword
    {
        Money,
        Morality,
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
        { "end" , TriggerKeyword.EventEnd },
        { "ends" , TriggerKeyword.EventEnd },
        { "start" , TriggerKeyword.EventStart },
        { "starts" , TriggerKeyword.EventStart },
        { "money" , TriggerKeyword.Money },
        { "weeks" , TriggerKeyword.Week },
        { "week" , TriggerKeyword.Week },
        { "morality" , TriggerKeyword.Morality },
        { "trigger", TriggerKeyword.DirectTrigger },
        { "upgrade_level" , TriggerKeyword.UpgradeLevel },
        { "upgrade_lvl" , TriggerKeyword.UpgradeLevel },
        { "level" , TriggerKeyword.UpgradeLevel },
    };

    // reads second word after reward
    public static readonly Dictionary<string, RewardKeyword> REWARD_KEYWORDS =
        new Dictionary<string, RewardKeyword>
    {
        { "morality" , RewardKeyword.Morality },
        { "money" , RewardKeyword.Money },
    };

}
