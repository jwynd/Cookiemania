using General_Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private const int LineLength = 38;

    // requires a list of 
    // tuple items -> name, voice line
    // and a dictionary of 
    // character name keys to -> tuple items -> display name, sprite
    // dictionary should be maintained in event manager script
    [SerializeField]
    private Canvas myCanvas = null;
    [SerializeField]
    private Image bgImage = null;
    [SerializeField]
    private Image charImage = null;
    [SerializeField]
    private TMP_Text charName = null;
    [SerializeField]
    private TMP_Text dialogueLine = null;
    [Range(0.001f, 0.3f)]
    [SerializeField]
    private float textDelay = 0.01f;
    [SerializeField]
    private bool useTestMode = false;
    [SerializeField]
    private List<Tuple<string, string>> testLines = new List<Tuple<string, string>> {
        new Tuple<string, string>("char_1", "line one"),
        new Tuple<string, string>("char_1", "line two is a lot longer than line one"),
        new Tuple<string, string>("char_2", "line three is also pretty long but ya know?"),
        new Tuple<string, string>("char_1", "line four isn't"),
    };
    [SerializeField]
    private Sprite[] testSprites = null;
    [SerializeField]
    private string[] testDisplayNames = null;
    [SerializeField]
    private List<Sprite> testBGs = new List<Sprite>();
    [SerializeField]
    private Dictionary<string, Tuple<string, Sprite>> testCharDictionary =
        new Dictionary<string, Tuple<string, Sprite>>()
    {
            { "char_1", new Tuple<string, Sprite>("Bob", null) },
            { "char_2", new Tuple<string, Sprite>("Frank", null) },
    };
    [HideInInspector]
    public delegate void OnComplete(string nextEvent);
    private OnComplete runOnComplete = null;
    // the current max the DialogueController can use
    [HideInInspector]
    public int CharacterMax { get; private set; } = 140;

    private ReadOnlyDictionary<string, Tuple<string, Sprite>> charDictionary;
    private List<Tuple<string, string>> lines = null;
    // whenever background is null, dont change; when background is a new sprite
    // change it
    private List<Sprite> backgrounds = null;
    private string nextEvent = "";
    private bool stillDisplayingText = false;
    private bool fastDisplayingText = false;
    private IEnumerator textDisplayer;
    private IEnumerator fastTextDisplayer;
    private Tuple<string, string> currentLine;

    public void Initialize(DialogueInfo dialogueInfo, OnComplete onComplete)
    {
        Initialize(dialogueInfo.Dialogues, onComplete, dialogueInfo.NextBranch,
            dialogueInfo.Backgrounds, EventManager.Instance.CharacterDictionary);
    }

    public void Initialize(
        List<Tuple<string, string>> dialogueLines,
        OnComplete onComplete,
        string nextEvent,
        List<Sprite> backgroundChanges = null,
        ReadOnlyDictionary<string, Tuple<string, Sprite>> characterDictionary = null)
    {
        // deep copy so we can pop without ruining data in event system
        lines = dialogueLines.ConvertAll(
            pair => new Tuple<string, string>(pair.Item1, pair.Item2));
        backgrounds = backgroundChanges.ConvertAll(bg => bg);
        this.nextEvent = nextEvent;
        runOnComplete = onComplete;
        if (characterDictionary != null)
        {
            // sprite should be changeable by event system
            // during dialogue if desired
            charDictionary = new ReadOnlyDictionary<string, Tuple<string, Sprite>>
                (characterDictionary);
        }
        if (charDictionary == null)
        {
            Debug.LogError("No character dictionary!\nUnable to create dialogue");
            return;
        }
        dialogueLine.text = "";
        bgImage.sprite = null;
        myCanvas.enabled = true;
        DisplayNextDialogue();
    }

    private void StartTextDisplay()
    {
        stillDisplayingText = true;
    }

    private void CompleteTextDisplay()
    {
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
                fastTextDisplayer = dialogueLine.TextDisplayer(currentLine.Item2, LineLength, textDelay * 0.2f, 
                    StartTextDisplay, CompleteTextDisplay, dialogueLine.text.Length);
                fastDisplayingText = true;
                stillDisplayingText = true;
                StartCoroutine(fastTextDisplayer);
            }
            return;
        }
        if (lines.Count < 1)
        {
            myCanvas.enabled = false;
            runOnComplete.Invoke(nextEvent);
            return;
        }
        currentLine = lines.PopFront();
        if (currentLine.Item2.Length > CharacterMax)
        {
            Debug.LogError("Next line is too long: " +
                currentLine.Item2.Length + " with max of " + CharacterMax);
            return;
        }
        if (backgrounds.Count > 0)
        {
            Sprite currentBG = backgrounds.PopFront();
            if (currentBG != null)
            {
                bgImage.sprite = currentBG;
            }
        }
        bgImage.color = bgImage.sprite == null ? 
                new Color(bgImage.color.r, bgImage.color.r, bgImage.color.r, 0) :
                new Color(bgImage.color.r, bgImage.color.r, bgImage.color.r, 1);
        
        if (charDictionary.TryGetValue(currentLine.Item1, out Tuple<string, Sprite> charInfo))
        {
            charName.text = charInfo.Item1;
            charImage.sprite = charInfo.Item2;
            // need to slowly display dialogue in async fn
            // will also want to set and unset the stillDisplayingText bool
            textDisplayer = dialogueLine.TextDisplayer(currentLine.Item2, LineLength, textDelay, 
                StartTextDisplay, CompleteTextDisplay);
            StartCoroutine(textDisplayer);
            //dialogueLine.text = line.Item2;
        }
        else
        {
            Debug.LogError("Character name: " + currentLine.Item1 + " not found in dict");
        }
    }

    private void Awake()
    {
        if (myCanvas == null)
        {
            Debug.LogError("Dialogue Controller: My canvas needs to be defined");
            return;
        }
        myCanvas.enabled = false;
        // waiting until explicit initialization if not in testing mode
#if UNITY_EDITOR
        if (useTestMode)
        {
            testCharDictionary["char_1"] =
                new Tuple<string, Sprite>(testDisplayNames[0], testSprites[0]);
            testCharDictionary["char_2"] =
                new Tuple<string, Sprite>(testDisplayNames[1], testSprites[1]);
            Initialize(testLines,
                (string next) => Debug.Log("test complete, next branch is: " + next),
                "last", testBGs, 
                new ReadOnlyDictionary<string, Tuple<string, Sprite>>(testCharDictionary));
        }
#endif
    }

}
