using General_Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// Basic use --> event system will pass a list of lines that the dialogue system is to use
//
// and will continue to spit out next piece of dialogue on click until complete. Calls
// the OnComplete function argument passed when out of dialogue

// Will need to assign dialogue to events... basically all this data needs to be hardcoded
// somewhere

public class DialogueController : MonoBehaviour
{
    // requires a list of 
    // tuple items -> name, voice line
    // and a dictionary of 
    // character name keys to -> tuple items -> display name, sprite
    // dictionary should be maintained in event manager script
    public Canvas myCanvas = null;
    public Image charImage = null;
    public TMP_Text charName = null;
    public TMP_Text dialogueLine = null;
    [Range(0.001f, 0.3f)]
    public float textDelay = 0.01f;
    public bool useTestMode = false;
    public List<Tuple<string, string>> testLines = new List<Tuple<string, string>> {
        new Tuple<string, string>("char_1", "line one"),
        new Tuple<string, string>("char_1", "line two is a lot longer than line one"),
        new Tuple<string, string>("char_2", "line three is also pretty long but ya know?"),
        new Tuple<string, string>("char_1", "line four isn't"),
    };
    public Sprite[] testSprites;
    public string[] testDisplayNames;
    public Dictionary<string, Tuple<string, Sprite>> testCharDictionary =
        new Dictionary<string, Tuple<string, Sprite>>()
    {
            { "char_1", new Tuple<string, Sprite>("Bob", null) },
            { "char_2", new Tuple<string, Sprite>("Frank", null) },
    };
    [HideInInspector]
    public delegate void OnComplete();
    public OnComplete runOnComplete = null;
    // the current max the DialogueController can use
    [HideInInspector]
    public int CharacterMax { get; private set; } = 96;

    private Dictionary<string, Tuple<string, Sprite>> charDictionary = null;
    private List<Tuple<string, string>> lines = null;
    private bool stillDisplayingText = false;
    private bool fastDisplayingText = false;
    private IEnumerator textDisplayer;
    private IEnumerator fastTextDisplayer;
    private Tuple<string, string> currentLine;

    public void Initialize(
        List<Tuple<string, string>> dialogueLines,
        OnComplete onComplete, 
        Dictionary<string, Tuple<string, Sprite>> characterDictionary = null)
    {
        // deep copy so we can pop without ruining data in event system
        lines = dialogueLines.ConvertAll(
            pair => new Tuple<string, string>(pair.Item1, pair.Item2));
        runOnComplete = onComplete;
        if (characterDictionary != null)
        {
            // sprite should be changeable by event system
            // during dialogue if desired
            charDictionary = characterDictionary;
        }
        if (charDictionary == null)
        {
            Debug.LogError("No character dictionary!\nUnable to create dialogue");
            return;
        }
        DisplayNextDialogue();
        myCanvas.enabled = true;
    }

    private IEnumerator TextDisplayer(string fullText, float delay, int startLetter = 0)
    {
        stillDisplayingText = true;
        for(int i = startLetter; i <= fullText.Length; i++)
        {
            dialogueLine.text = fullText.Substring(0, i);
            yield return new WaitForSecondsRealtime(delay);
        }
        stillDisplayingText = false;
        fastDisplayingText = false;
    }

    public void DisplayNextDialogue()
    {
        if (stillDisplayingText)
        {
            if (!fastDisplayingText)
            {
                StopCoroutine(textDisplayer);
                fastTextDisplayer = TextDisplayer(currentLine.Item2, textDelay * 0.2f, dialogueLine.text.Length);
                fastDisplayingText = true;
                stillDisplayingText = true;
                StartCoroutine(fastTextDisplayer);
            }
            return;
        }
        if (lines.Count < 1)
        {
            myCanvas.enabled = false;
            runOnComplete.Invoke();
            return;
        }
        currentLine = lines.PopFront();
        if (currentLine.Item2.Length > CharacterMax)
        {
            Debug.LogError("Next line is too long: " + 
                currentLine.Item2.Length + " with max of " + CharacterMax);
            return;
        }
        if (charDictionary.TryGetValue(currentLine.Item1, out Tuple<string, Sprite> charInfo))
        {
            charName.text = charInfo.Item1;
            charImage.sprite = charInfo.Item2;
            // need to slowly display dialogue in async fn
            // will also want to set and unset the stillDisplayingText bool
            textDisplayer = TextDisplayer(currentLine.Item2, textDelay);
            StartCoroutine(textDisplayer);
            //dialogueLine.text = line.Item2;
        }
        else
        {
            Debug.LogError("Character name: " + currentLine.Item1 + " not found in dict");
        }
    }

    void Start()
    {
        if (myCanvas == null)
        {
            Debug.LogError("Dialogue Controller: My canvas needs to be defined");
            return;
        }
        myCanvas.enabled = false;
        // waiting until explicit initialization if not in testing mode
        if (useTestMode) 
        {
            testCharDictionary["char_1"] =
                new Tuple<string, Sprite>(testDisplayNames[0], testSprites[0]);
            testCharDictionary["char_2"] =
                new Tuple<string, Sprite>(testDisplayNames[1], testSprites[1]);
            Initialize(testLines, () => Debug.Log("test complete"), 
                testCharDictionary);
        }
    }
}
