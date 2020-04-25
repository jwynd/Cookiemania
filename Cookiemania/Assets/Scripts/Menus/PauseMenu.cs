using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this is required but abstract classes can't be added
//so manual adding will be required
//[RequireComponent(typeof(General_Input))]

using static General_Utilities.Children;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    protected GameObject settingsPrefab;
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
    protected SettingsMenu settings;


    private void Awake()
    {
        normalTimeScale = Time.timeScale;
        SetupSettings();
        Resume();
    }

    private void SetupSettings()
    {
        settings = settingsPrefab.GetComponent<SettingsMenu>();
        settings.enableOnDestroy = new List<MonoBehaviour> { this };
        List<GameObject> addToActivate = new List<GameObject>();
        foreach (Transform child in myPanel.transform)
        {
            addToActivate.Add(child.gameObject);
        }
        settings.activateOnDestroy = addToActivate;
        settingsPrefab.SetActive(false);
    }

    private void Start()
    {
        levelController = General_LevelTransition.Instance;
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
        //only one input per frame
        var vert = input.Vertical;
        if (input.Resume > 0f)
        {
            Resume();
        }
        else if (input.Exit > 0)
        {
            Exit();
        }
        else if (input.Settings > 0f)
        {
            Settings();
        }
        else if (vert > 0)
        {
            Up();
        }
        else if (vert < 0)
        {
            Down();
        }
        else if (input.Enter > 0)
        {
            Enter();
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
        markedButton = -1;
        Up();
        menuActive = true;
        transform.SetActiveChildren(menuActive);
        settingsPrefab.SetActive(false);
    }

    public void Resume()
    {
        menuActive = false;
        settingsPrefab.SetActive(false);
        transform.SetActiveChildren(menuActive);
        Time.timeScale = normalTimeScale;
    }

    public void Settings()
    {
        myPanel.transform.SetActiveChildren(false);
        enabled = false;
        settings.enabled = true;
        settingsPrefab.SetActive(true);
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

    private void OnEnable()
    {
        settings.enabled = false;
    }
}
