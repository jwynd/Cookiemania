using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueInfo
{
    // item 1 is the characters unique name, item 2 is their line
    public List<Tuple<string, string>> Dialogues;
    public DialogueController.OnComplete RunOnComplete;

    public List<Sprite> Backgrounds;
    // if true this event will exit the event
    public bool ExitsEvent = false;
    // all the events that this dialogue sequence will immediately trigger
    // in order of trigger
    public HashSet<string> DirectlyTriggeredEvents = new HashSet<string>();
    public string NextBranch;
    public string UniqueName;

    public DialogueInfo(
        string uniqueName,
        DialogueController.OnComplete runOnComplete = null,
        string nextEvent = "",
        List<Tuple<string, string>> dialogues = null,
        List<Sprite> backgrounds = null)
    {
        this.Dialogues = dialogues != null ?
            dialogues : new List<Tuple<string, string>>();
        this.RunOnComplete = runOnComplete;
        this.NextBranch = nextEvent;
        this.UniqueName = uniqueName;
        this.Backgrounds = backgrounds != null ?
            backgrounds : new List<Sprite>();
    }

    // if no unique name is given, argument will be provided as most recent talking
    // character
    public void AddDialogue(string dialogue, string charUniqueName = "")
    {
        if (charUniqueName == "")
        {
            charUniqueName = Dialogues.Last().Item1;
        }
        Dialogues.Add(new Tuple<string, string>(charUniqueName, dialogue));
        while (Dialogues.Count < Backgrounds.Count)
        {
            Backgrounds.Add(null);
        }
    }

    // obviously only one background per dialogue line
    // so add background only effects most recent line of dialogue added
    // so background should be defined before the line of dialogue
    // exception is the first background added to a dialogueinfo
    public void AddBackground(Sprite background)
    {
        if (Backgrounds.Count < 1)
        {
            Backgrounds.Add(background);
            return;
        }
        Backgrounds[Backgrounds.Count - 1] = background;
    }
}