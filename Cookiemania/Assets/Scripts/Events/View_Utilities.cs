using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public static class View_Utilities
{
    public static IEnumerator TextDisplayer(this TMP_Text textBox,
       string fullText, float delay, Action startingAction, Action endingAction, int resumePoint = 0)
    {
        startingAction.Invoke();
        if (EventManager.Instance)
            fullText = EventManager.Instance.GetDialogueWithOverwrites(fullText);
        yield return new WaitForSecondsRealtime(delay);
        // use resume point here for correct implementation
        for (int i = resumePoint; i <= fullText.Length; i++)
        {
            // 0 is correct implementation (only want sped up textdisplayer in use case
            // where we have start letter, don't want to start over)
            string temp = fullText.Substring(0, i);
            // this is rich text tagged so only want to do slow display of string
            // when we have full tags
            if (temp.Count(f => f == '<') == temp.Count(f => f == '>'))
            {
                textBox.text = temp;
                yield return new WaitForSecondsRealtime(delay);
            }
        }
        // in case error was made and text has rich text tags
        // 0 is correct
        textBox.text = fullText.Substring(0, fullText.Length);
        endingAction.Invoke();
    }
}
