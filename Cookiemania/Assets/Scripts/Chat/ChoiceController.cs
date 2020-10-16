using System.Collections.Generic;
using System;
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
    private Image bgImage = null;
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
    private Sprite testBG;
    [SerializeField]
    private string testChoicePrompt = "";
    [SerializeField]
    private List<string> testChoices = new List<string>();
    [Serializable]
    public class RewardTupleStandIn
    {
        public ScriptConstants.RewardKeyword rewardType;
        public int amount;

        public RewardTupleStandIn(ScriptConstants.RewardKeyword item1, int item2)
        {
            rewardType = item1;
            amount = item2;
        }
    }
    [Serializable]
    public class RewardListStandIn
    {
        public List<RewardTupleStandIn> rewards = new List<RewardTupleStandIn>();
        public RewardListStandIn(List<RewardTupleStandIn> items)
        {
            rewards = items;
        }
    }

    [SerializeField]
    private List<RewardListStandIn> testRewards = 
        new List<RewardListStandIn>();
    [SerializeField]
    private List<string> testNextBranches = 
        new List<string>();

    private List<List<Tuple<ScriptConstants.RewardKeyword, int>>> rewards = 
        new List<List<Tuple<ScriptConstants.RewardKeyword, int>>>();
    private List<string> nextBranches = 
        new List<string>();

    [HideInInspector]
    public delegate void OnComplete(string nextBranch, int choiceNumber, 
        List<Tuple<ScriptConstants.RewardKeyword, int>> rewardList );
    private OnComplete runOnComplete = null;

    private List<Button> choiceButtons = new List<Button>();
    private float originalAlpha;

    void Awake()
    {
        originalAlpha = bgImage.color.a;
        choiceButtons.Add(choice1);
        choiceButtons.Add(choice2);
        choiceButtons.Add(choice3);
        choiceButtons.Add(choice4);
        EnableObjects(false);
#if UNITY_EDITOR
        if (useTestMode)
        {
            var altTestRewards = new List<List<Tuple<ScriptConstants.RewardKeyword, int>>>();
            foreach (var list in testRewards)
            {
                var altInnerList = new List<Tuple<ScriptConstants.RewardKeyword, int>>();
                foreach (var tuple in list.rewards)
                {
                    altInnerList.Add(new Tuple<ScriptConstants.RewardKeyword, int>(
                        tuple.rewardType, tuple.amount));
                }
                altTestRewards.Add(altInnerList);
            }
            Initialize(testCharName, testCharImage, testChoicePrompt, testChoices,
                altTestRewards, testNextBranches, 
                (string nextB, int v, List<Tuple<ScriptConstants.RewardKeyword, int>> rewards) => 
                    Debug.Log("test complete, " + v + " was selected, with rewards " + 
                    string.Join(" ", rewards) + "\nnext branch is: " + nextB),
                testBG);
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
        string choicePrompt, List<string> choices, 
        List<List<Tuple<ScriptConstants.RewardKeyword, int>>> rewards, 
        List<string> nextEvents,
        OnComplete onComplete, 
        Sprite background = null)
    {
        if (choices.Count != rewards.Count ||
            choices.Count != nextEvents.Count)
        {
            throw new Exception("rewards list and choices list must be same size");
        }
        EnableObjects(false);
        charName.text = cName;
        charImage.sprite = cImage;
        dialogueLine.text = choicePrompt;
        runOnComplete = onComplete;
        bgImage.sprite = background;
        // not manipulating this data :> shallow copy
        foreach (var item in rewards)
        {
            this.rewards.Add(item);
        }
        this.nextBranches = nextEvents;
        bgImage.color = bgImage.sprite == null ?
                new Color(bgImage.color.r, bgImage.color.r, bgImage.color.r, 0) :
                new Color(bgImage.color.r, bgImage.color.r, bgImage.color.r, originalAlpha);
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
        runOnComplete.Invoke(nextBranches[v - 1], v, rewards[v - 1]);
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
