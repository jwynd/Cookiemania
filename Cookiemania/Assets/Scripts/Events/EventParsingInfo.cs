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

    public void ResetForNextEvent()
    {
        EventInfo = null;
        CharacterInfo = null;
        BackgroundInfo = null;
        TrimmedLine.Clear();
        ChoiceDialoguesToMultiWrite.Clear();
        IsChoiceIsChoiceDialogue = 
            new Tuple<bool, bool>(false, false);
    }

    public string GetLowercaseWord(int index)
    {
        ThrowOnIndexFail(index);
        return TrimmedLine[index].ToLowerInvariant().Trim();
    }

    public int GetParsedInt(int index)
    {
        return int.Parse(GetLowercaseWord(index));
    }

    private void ThrowOnIndexFail(int index)
    {
        if (index >= TrimmedLine.Count)
        {
            throw new Exception("retrieval index too high: " + index + " for line: "
                + string.Join(", ", TrimmedLine));
        }
    }
}
