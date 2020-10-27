using General_Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

    public delegate void ActionRef<T1, T2, T3, T4>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4);


    // the choice bools (tuple bool) should be named (insideChoice, insideChoiceDialogueBranch)
    public static readonly Dictionary<BaseKeyword, ActionRef<EventInfo, List<string>,
        List<string>, Tuple<bool, bool>>> KeywordActions =
        new Dictionary<BaseKeyword, ActionRef<EventInfo, List<string>,
        List<string>, Tuple<bool, bool>>>
        {
            {BaseKeyword.BackgroundChange, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(BackgroundChangeAction) },
            {BaseKeyword.Branch, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(BranchAction) },
            {BaseKeyword.BranchEnd, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(BranchEndAction) },
            {BaseKeyword.BranchStart, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(BranchStartAction) },
            {BaseKeyword.Choice, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(ChoiceAction) },
            {BaseKeyword.ChoiceEnd,new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(ChoiceEndAction) },
            {BaseKeyword.Event, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(EventAction) },
            {BaseKeyword.EventEarlyEnd, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(EventEarlyEndAction) },
            {BaseKeyword.EventEnd, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(EventEndAction) },
            {BaseKeyword.EventReward, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(EventRewardAction) },
            {BaseKeyword.Reward, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(RewardAction) },
            {BaseKeyword.Trigger, new ActionRef<EventInfo, List<string>,
                List<string>, Tuple<bool, bool>>(TriggerAction) },
        };

    // item1 of choice bools is whether in choice, item2 is whether in a choice branch's dialogue
    public static void BackgroundChangeAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        throw new NotImplementedException();
    }

    public static void BranchAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        choiceBools = new Tuple<bool, bool>(true, false);
        eventInfo.GetLastChoice().AddChoice("");
    }

    public static void BranchEndAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        return;
    }

    public static void BranchStartAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        if (!choiceBools.Item1)
        {
            throw new Exception("must be in a choice to create choice branches");
        }
        // gets flipped to true for choice declaration completion as the next
        // dialogues must be the dialogues associated with this choice
        ChoiceDeclarationComplete(eventInfo);
        choiceBools = new Tuple<bool, bool>(true, true);
        line.PopFront();
        choiceBranches.Clear();
        var choice = eventInfo.GetLastChoice();
        foreach (var str in line)
        {
            // actual index is 1 less
            var index = int.Parse(str) - 1;
            if (choice.ChoiceDialogueDictionary.TryGetValue(index, out string name))
            {
                choiceBranches.Add(name);
            }
            else
            {
                throw new Exception(index + " not found in choice dictionary");
            }
        }
    }

    public static void ChoiceAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        choiceBools = new Tuple<bool, bool>(true, false);
        eventInfo.AddChoice(new ChoiceInfo(eventInfo.BranchID.ToString()));
        // need to add this choice's branch name to whatever the previous dialogue branch was
        eventInfo.SetNextBranch(eventInfo.GetLastDialogue().UniqueName,
            eventInfo.GetLastChoice().UniqueName);
    }

    public static void ChoiceEndAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        BranchEndAction(ref eventInfo, ref line, ref choiceBranches, ref choiceBools);
        choiceBools = new Tuple<bool, bool>(false, false);
        // also create next dialogue here and point choices to that dialogue
        var choice = eventInfo.GetLastChoice();
        eventInfo.AddDialogue(new DialogueInfo(eventInfo.BranchID.ToString()));
        var nextBranch = eventInfo.GetLastDialogue().UniqueName;
        foreach (var ch in choice.ChoiceDialogueDictionary.Values)
        {
            eventInfo.SetNextBranch(ch, nextBranch);
        }
    }

    public static void EventAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        if (line.Count < 2)
        {
            throw new Exception("event needs a name on declaration line");
        }
        eventInfo = new EventInfo(line[1].ToLowerInvariant().Trim());
    }

    public static void EventEarlyEndAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        if (choiceBools.Item1)
            eventInfo.MultiEarlyExitWrite(choiceBranches);
        else
            eventInfo.GetLastDialogue().ExitsEvent = true;
        BranchEndAction(ref eventInfo, ref line, ref choiceBranches, ref choiceBools);
    }

    public static void EventEndAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        eventInfo.GetLastDialogue().NextBranch = EventInfo.LAST_BRANCH;
        eventInfo.GetLastDialogue().ExitsEvent = true;
        if (!EventManager.Instance.AddEvent(eventInfo))
        {
            throw new Exception("cannot add duplicate event name to event " +
                "dictionary: " + eventInfo.UniqueName);
        }
        eventInfo = null;
    }

    public static void EventRewardAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        if (choiceBools.Item1)
        {
            throw new Exception("event complete rewards may not be declared inside a choice");
        }
        eventInfo.EventCompleteReward.Add(GetRewardTuple(eventInfo, line));
    }

    public static void RewardAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        if (!choiceBools.Item1)
        {
            eventInfo.PrintInformation();
            throw new Exception("can only provide normal rewards inside a choice" +
                " declaration");
        }
        var reward = GetRewardTuple(eventInfo, line);
        eventInfo.GetLastChoice().AddReward(reward.Item1, reward.Item2);
    }

    private static Tuple<RewardKeyword, int> GetRewardTuple(EventInfo eventInfo, List<string> line)
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

    public static void TriggerAction(ref EventInfo eventInfo, ref List<string> line,
        ref List<string> choiceBranches, ref Tuple<bool, bool> choiceBools)
    {
        var eventName = line[1].ToLowerInvariant().Trim();
        if (choiceBools.Item1)
        {
            eventInfo.MultiEventTriggerWrite(choiceBranches, eventName);
        }
        else
        {
            // obviously the trigger needs a second word: the name of the event getting triggered
            eventInfo.GetLastDialogue().DirectlyTriggeredEvents.Add(eventName);
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
}
