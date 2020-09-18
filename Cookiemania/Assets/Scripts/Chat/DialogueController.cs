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
    public bool useTestMode = false;
    public List<Tuple<string, string>> testLines = new List<Tuple<string, string>> {
        new Tuple<string, string>("char_1", "line one"),
        new Tuple<string, string>("char_1", "line two"),
        new Tuple<string, string>("char_2", "line three"),
        new Tuple<string, string>("char_1", "line four"),
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
    private bool stillDisplayingText;

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

    public void DisplayNextDialogue()
    {
        if (stillDisplayingText)
        {
            // dramatically increase lineDisplaySpeed
        }
        if (lines.Count < 1)
        {
            myCanvas.enabled = false;
            runOnComplete.Invoke();
            return;
        }
        Tuple<string, string> line = lines.PopFront();
        if (line.Item2.Length > CharacterMax)
        {
            Debug.LogError("Next line is too long: " + 
                line.Item2.Length + " with max of " + CharacterMax);
            return;
        }
        if (charDictionary.TryGetValue(line.Item1, out Tuple<string, Sprite> charInfo))
        {
            charName.text = charInfo.Item1;
            charImage.sprite = charInfo.Item2;
            // need to slowly display dialogue in async fn
            // will also want to set and unset the stillDisplayingText bool
            dialogueLine.text = line.Item2;
        }
        else
        {
            Debug.LogError("Character name: " + line.Item1 + " not found in dict");
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
