using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceController : MonoBehaviour
{
    // the choice controller directly accepts the sprite, character name
    // dialogue and choices when initialize is run
    // supports 2-4 choices, runs a callback function with argument 
    // as to selected choice (1,2,3 or 4)
    // disables self on callback
    public Canvas myCanvas = null;
    public Image charImage = null;
    public TMP_Text charName = null;
    public TMP_Text dialogueLine = null;
    public Button choice1 = null;
    public Button choice2 = null;
    public Button choice3 = null;
    public Button choice4 = null;
    public bool useTestMode = false;
    public Sprite testCharImage = null;
    public string testCharName = "";
    public string testChoicePrompt = "";
    public List<string> testChoices = new List<string>();

    private List<Button> choiceButtons = new List<Button>();

    [HideInInspector]
    public delegate void OnComplete(int choiceNumber);
    private OnComplete runOnComplete = null;

    void Start()
    {
        choiceButtons.Add(choice1);
        choiceButtons.Add(choice2);
        choiceButtons.Add(choice3);
        choiceButtons.Add(choice4);
        EnableObjects(false);
#if UNITY_EDITOR
        if (useTestMode)
        {
            Initialize(testCharName, testCharImage, testChoicePrompt, testChoices,
                (int v) => Debug.Log("test complete, " + v + " was selected"));
        }
#endif
    }

    private void EnableObjects(bool enable)
    {
        myCanvas.enabled = enable;
        choice1.enabled = enable;
        choice2.enabled = enable;
        choice3.enabled = enable;
        choice4.enabled = enable;
    }

    public void Initialize(string cName, Sprite cImage, 
        string choicePrompt, List<string> choices, OnComplete onComplete)
    {
        EnableObjects(false);
        charName.text = cName;
        charImage.sprite = cImage;
        dialogueLine.text = choicePrompt;
        runOnComplete = onComplete;
        for (int i = 0; i < choices.Count; i++)
        {
            choiceButtons[i].GetComponentInChildren<TMP_Text>().
                text = choices[i];
            choiceButtons[i].enabled = true;
        }
        myCanvas.enabled = true;
    }
    private void EndChoice(int v)
    {
        EnableObjects(false);
        runOnComplete.Invoke(v);
    }

    // not great, but it was easier to give every button its own callback fn
    public void ChoiceOne()
    {
        EndChoice(1);
    }

    public void ChoiceTwo()
    {
        EndChoice(2);
    }
    public void ChoiceThree()
    {
        EndChoice(3);
    }
    public void ChoiceFour()
    {
        EndChoice(4);
    }
}
