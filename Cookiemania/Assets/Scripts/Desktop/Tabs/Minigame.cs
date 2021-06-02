using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Minigame : MonoBehaviour
{
    [SerializeField]
    protected GameObject minigameUIPrefab = null;

    [SerializeField]
    protected string minigame1scene = null;
    [SerializeField]
    protected string minigame2scene = null;
    [SerializeField]
    protected GameObject homeTab = null;

    protected GameObject minigameRef = null;
    protected GameObject websiteCustomizationRef = null;
    protected Canvas minigameCanvas = null;
    protected Canvas customizationCanvas = null;

    protected void MinigameOneStart()
    {

        General_LevelTransition.Instance.ToMinigame(minigame1scene);

    }

    protected void MinigameTwoStart()
    {
        General_LevelTransition.Instance.ToMinigame(minigame2scene);
    }

    protected void ReturnFunction()
    {
        homeTab.GetComponent<General_TabButton>().Click();
    }

    private void Awake()
    {
        minigameRef = Instantiate(minigameUIPrefab);
        //dont parent
        minigameCanvas = minigameRef.GetComponent<Canvas>();
        minigameCanvas.gameObject.SetActive(false);
        minigameCanvas.enabled = false;
        MinigameUI setRef = minigameRef.GetComponent<MinigameUI>();
        setRef.SetGame1Listener(MinigameOneStart);
        setRef.SetGame2Listener(MinigameTwoStart);
        setRef.SetReturnListener(ReturnFunction);
    }

    private void Start()
    {
        General_LevelTransition.Instance.DisableOnLevelChange.Add(minigameRef);
        minigameCanvas.transform.SetParent(SiteCanvas.Instance.transform);
    }

    private void OnEnable()
    {
        //go to website instead
        if (!minigameCanvas)
            return;
        minigameCanvas.gameObject.SetActive(true);
        minigameCanvas.enabled = true;

    }

    private void OnDisable()
    {
        if (!minigameCanvas)
            return;
        minigameCanvas.gameObject.SetActive(false);
        minigameCanvas.enabled = false;

    }

}
