﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public static class View_Utilities
{
    public static IEnumerator TextDisplayer(this TMP_Text textBox,
       string fullText, int lineLength, float delay, Action startingAction, Action endingAction, int resumePoint = 0)
    {
        startingAction.Invoke();
        var yieldDelay = new WaitForSecondsRealtime(delay);
        if (EventManager.Instance)
            fullText = EventManager.Instance.GetDialogueWithOverwrites(fullText);
        if (fullText.Length > lineLength)
        {
            fullText = SplitTextIntoLines(fullText, lineLength);
        }
        yield return yieldDelay;
        int startCount = 0;
        int endCount = 0;
        for (int i = 0; i < resumePoint && i < fullText.Length; i++)
        {
            if (fullText[i] == '<')
                startCount++;
            else if (fullText[i] == '>')
                endCount++;
        }
        // use resume point here for correct implementation
        for (int i = resumePoint; i < fullText.Length; i++)
        {
            // 0 is correct implementation (only want sped up textdisplayer in use case
            // where we have start letter, don't want to start over)
            if (fullText[i] == '<')
                startCount++;
            else if (fullText[i] == '>')
                endCount++;
            string temp = fullText.Substring(0, i);
            // this is rich text tagged so only want to do slow display of string
            // when we have full tags
            if (startCount == endCount)
            {
                textBox.text = temp;
                yield return yieldDelay;
            }
        }
        // in case error was made and text has rich text tags
        // 0 is correct
        textBox.text = fullText;
        endingAction.Invoke();
    }

    public static string SplitTextIntoLines(string fullText, int lineLength)
    {
        // need to split by word, then split by char count of list of words
        var words = fullText.Split(' ');
        var lines = new List<string>();
        var charCount = lineLength + 1;
        var lineIndex = -1;
        foreach(var word in words)
        {
            charCount += word.Length + 1;
            if (charCount >= lineLength)
            {
                lines.Add(word);
                lineIndex++;
                charCount = word.Length + 1;
            }
            else
            {
                lines[lineIndex] += " " + word;
            }
        }
        fullText = "";
        foreach (var line in lines)
        {
            fullText += line + "\n";
        }

        return fullText;
    }
}
