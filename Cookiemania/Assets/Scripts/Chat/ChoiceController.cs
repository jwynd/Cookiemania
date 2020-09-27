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
    [SerializeField]
    private Canvas myCanvas = null;
    [SerializeField]
    private Image charImage = null;
    [SerializeField]
    private TMP_Text charName = null;
    [SerializeField]
    private TMP_Text dialogueLine = null;
    [SerializeField]
    private Button choice1 = null;
    [SerializeField]
    private Button choice2 = null;
    [SerializeField]
    private Button choice3 = null;
    [SerializeField]
    private Button choice4 = null;
    [SerializeField]
    private bool useTestMode = false;
    [SerializeField]
    private Sprite testCharImage = null;
    [SerializeField]
    private string testCharName = "";
    [SerializeField]
    private string testChoicePrompt = "";
    [SerializeField]
    private List<string> testChoices = new List<string>();

    [HideInInspector]
    public delegate void OnComplete(int choiceNumber);
    private OnComplete runOnComplete = null;

    private List<Button> choiceButtons = new List<Button>();

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
        choice1.gameObject.SetActive(enable);
        choice2.gameObject.SetActive(enable);
        choice3.gameObject.SetActive(enable);
        choice4.gameObject.SetActive(enable);
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
            choiceButtons[i].gameObject.SetActive(true);
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
