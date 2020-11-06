﻿using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

using static Parsing_Utilities;

public class ChoiceController : MonoBehaviour
{
    public const int MAX_CHOICES_SUPPORTED = 4;
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
    private Sprite testBG = null;
    [SerializeField]
    private string testChoicePrompt = "";
    [SerializeField]
    private List<string> testChoices = new List<string>();

    [Serializable]
    public class RewardTupleStandIn
    {
        public RewardKeyword rewardType;
        public int amount;

        public RewardTupleStandIn(RewardKeyword item1, int item2)
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

    private List<List<Tuple<RewardKeyword, int>>> rewards = 
        new List<List<Tuple<RewardKeyword, int>>>();
    private List<string> nextBranches = 
        new List<string>();
    // the choice texts
    private List<string> choices = new List<string>();
    // used to get actual choice made
    private List<int> choiceOrderAlias = new List<int>();
    [HideInInspector]
    public delegate void OnComplete(string nextBranch,
        string choicePrompt, 
        string choiceMade,
        List<Tuple<RewardKeyword, int>> rewardList );
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
            var altTestRewards = new List<List<Tuple<RewardKeyword, int>>>();
            foreach (var list in testRewards)
            {
                var altInnerList = new List<Tuple<RewardKeyword, int>>();
                foreach (var tuple in list.rewards)
                {
                    altInnerList.Add(new Tuple<RewardKeyword, int>(
                        tuple.rewardType, tuple.amount));
                }
                altTestRewards.Add(altInnerList);
            }
            Initialize(testCharName, testCharImage, testChoicePrompt, testChoices,
                altTestRewards, testNextBranches, 
                (string nextB, string v, string c, List<Tuple<RewardKeyword, int>> rewards) => 
                    Debug.Log("test complete for choice: " + c + ", " + v + " was selected, with rewards " + 
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

    public void Initialize(ChoiceInfo choiceInfo, OnComplete onComplete)
    {
        Initialize(choiceInfo.CharacterName, choiceInfo.CharacterImage,
            choiceInfo.Prompt, choiceInfo.Choices, choiceInfo.Rewards,
            choiceInfo.ChoiceDialogueDictionary.Values.ToList(),
            onComplete, choiceInfo.Background);
    }

    public void Initialize(string cName, Sprite cImage, 
        string choicePrompt, List<string> choices, 
        List<List<Tuple<RewardKeyword, int>>> rewards, 
        List<string> nextEvents,
        OnComplete onComplete, 
        Sprite background = null)
    {
        if (choices.Count != rewards.Count ||
            choices.Count != nextEvents.Count)
        {
            throw new Exception("rewards list and choices list must be same size");
        }

        if (choices.Count > MAX_CHOICES_SUPPORTED)
        {
            throw new Exception("maximum amount of choices is " + MAX_CHOICES_SUPPORTED);
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
        this.choices = choices;
        bgImage.color = bgImage.sprite == null ?
                new Color(bgImage.color.r, bgImage.color.r, bgImage.color.r, 0) :
                new Color(bgImage.color.r, bgImage.color.r, bgImage.color.r, originalAlpha);
        choiceOrderAlias = Enumerable.Range(0, choices.Count).ToList();
        // shuffling list
        System.Random rnd = new System.Random();
        choiceOrderAlias = choiceOrderAlias.Select(x =>
            new { value = x, order = rnd.Next() })
            .OrderBy(x => x.order).Select(x => x.value).ToList();
        for (int i = 0; i < this.choices.Count; i++)
        {
            choiceButtons[i].GetComponentInChildren<TMP_Text>().
                text = this.choices[choiceOrderAlias[i]];
            choiceButtons[i].gameObject.SetActive(true);
        }
        StartCoroutine(UpdateCanvas());
    }


    // layout groups in unity are dumb af and need to be told to refresh for some 
    // reason
    private IEnumerator UpdateCanvas()
    {
        yield return new WaitForEndOfFrame();
        // why is this necessary?
        foreach (var comp in myCanvas.GetComponentsInChildren<VerticalLayoutGroup>())
        {
            comp.childScaleHeight = false;
            comp.childScaleHeight = true;
        }
        // because it forces it to re-evaluate it seems
        foreach (var comp in myCanvas.GetComponentsInChildren<HorizontalLayoutGroup>())
        {
            comp.childScaleHeight = false;
            comp.childScaleHeight = true;
        }
        yield return new WaitForEndOfFrame();
        myCanvas.enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    private void EndChoice(int v)
    {
        EnableObjects(false);
        Debug.Log("selected choice #" + v.ToString());
        runOnComplete.Invoke(nextBranches[v],  
            dialogueLine.text, choices[v], rewards[v]);
    }

    // not great, but it was easier to give every button its own callback fn
    public void ChoiceOne()
    {
        EndChoice(choiceOrderAlias[0]);
    }

    public void ChoiceTwo()
    {
        EndChoice(choiceOrderAlias[1]);
    }
    public void ChoiceThree()
    {
        EndChoice(choiceOrderAlias[2]);
    }
    public void ChoiceFour()
    {
        EndChoice(choiceOrderAlias[3]);
    }
}
