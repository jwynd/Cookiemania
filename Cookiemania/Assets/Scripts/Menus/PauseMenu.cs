using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//this is required but abstract classes can't be added
//so manual adding will be required
//[RequireComponent(typeof(General_Input))]
public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    protected General_InputMenu input;
    [SerializeField]
    protected GameObject markerObj;
    [SerializeField]
    protected GameObject myPanel;
    [Tooltip("Buttons in visual order, top to bottom")]
    [SerializeField]
    protected List<Button> myChildButtons = new List<Button>();
    protected int markedButton = 0;
    protected bool menuActive = false;
    protected float normalTimeScale;
    protected General_LevelTransition levelController;

    private void Awake()
    {
        normalTimeScale = Time.timeScale;
        Resume();
    }

    private void Start()
    {
        GameObject levelC = GameObject.Find("LevelController");
        if (levelC != null)
        {
            levelController = levelC.GetComponent<General_LevelTransition>();
        }
    }

    protected void ActivateChildren(bool active)
    {
        myPanel.SetActive(active);
        foreach (var but in myChildButtons)
        {
            but.gameObject.SetActive(active);
        }
    }

    private void Update()
    {
        if (!menuActive && input.OpenMenu > 0f)
        {
            Debug.Log("clicked");
            Pause();
            return;
        }
        if (!menuActive)
        {
            return;
        }
        CheckMenuInputs();
    }

    private void CheckMenuInputs()
    {
        if (input.Resume > 0f)
        {
            Resume();
            return;
        }
        if (input.Exit > 0)
        {
            Exit();
            return;
        }
        if (input.Settings > 0f)
        {
            Settings();
            return;
        }
        var vert = input.Vertical;
        if (vert > 0)
        {
            Up();
            return;
        }
        if (vert < 0)
        {
            Down();
            return;
        }
        if (input.Enter > 0)
        {
            Enter();
            return;
        }
    }

    protected void Enter()
    {
        if (markedButton < 0)
        {
            return;
        }
        myChildButtons[markedButton].onClick.Invoke();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        menuActive = true;
        ActivateChildren(menuActive);
    }

    public void Resume()
    {
        menuActive = false;
        ActivateChildren(menuActive);
        Time.timeScale = normalTimeScale;
    }

    public void Settings()
    {

    }

    public void Exit()
    {
        Resume();
        if (levelController)
        {
            levelController.returnDesktop();
        }
        Debug.LogWarning("No level transition object in game");
    }

    //pause menu only configured to go up and down rn
    protected void Up()
    {
        int t = markedButton;
        markedButton = Mathf.Max(0, markedButton - 1);
        //move button up if changed value
        if (t != markedButton)
        {
            //move marker up
            markerObj.transform.position = new Vector3(markerObj.transform.position.x,
                                                       myChildButtons[markedButton].transform.position.y,
                                                       markerObj.transform.position.z);
        }
    }

    protected void Down()
    {
        int t = markedButton;
        markedButton = Mathf.Min(myChildButtons.Count - 1, markedButton + 1);
        if (t != markedButton)
        {
            //move marker down
            markerObj.transform.position = new Vector3(markerObj.transform.position.x,
                                                       myChildButtons[markedButton].transform.position.y,
                                                       markerObj.transform.position.z);
        }
    }
}
