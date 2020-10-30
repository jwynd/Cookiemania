using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

// to be used while parsing from a script to create an event info
[Serializable]
public class EventParsingInfo
{
    public CharacterInfo CharacterInfo = null;
    public BackgroundInfo BackgroundInfo = null;
    public Tuple<bool, bool> IsChoiceIsChoiceDialogue = 
       new Tuple<bool, bool>(false, false);
    public List<string> ChoiceDialoguesToMultiWrite = 
       new List<string>();
    public Dictionary<string, EventInfo> EventInfos = 
       new Dictionary<string, EventInfo>();
    public EventInfo EventInfo = null;
    public List<string> TrimmedLine = new List<string>();
    public int MaxChoices = 0;
    
    public EventParsingInfo()
    {

    }
}
