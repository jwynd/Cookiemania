using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static General_Utilities.Children;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    protected GameObject settingsPrefab;
    [SerializeField]
    protected GameObject savePrefab;
    [SerializeField]
    protected GameObject markerObj;
    [SerializeField]
    protected GameObject myPanel;
    [Tooltip("Buttons in visual order, top to bottom")]
    [SerializeField]
    protected List<Button> myChildButtons = new List<Button>();
    [SerializeField]
    protected Button saveButton = null;
    protected int markedButton = 0;
    protected bool menuActive = false;
    protected float normalTimeScale;
    protected General_LevelTransition levelController;
    private GameObject charCustom;
    protected SettingsMenu settings;
    protected SaveMenu save;

    public static PauseMenu Instance { get; protected set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        normalTimeScale = Time.timeScale;
        SetupSettings();
        SaveSettings();
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

    private void SaveSettings()
    {
        save = savePrefab.GetComponent<SaveMenu>();
        save.enableOnDestroy = new List<MonoBehaviour> { this };
        List<GameObject> addToActivate = new List<GameObject>();
        foreach (Transform child in myPanel.transform)
        {
            addToActivate.Add(child.gameObject);
        }
        save.activateOnDestroy = addToActivate;
        savePrefab.SetActive(false);
    }



    private void Start()
    {
        levelController = General_LevelTransition.Instance;
        var possibleChar = FindObjectOfType<CharacterContentManager>();
        if (possibleChar)
            charCustom = possibleChar.gameObject;
        InputAxes.Instance.Escape.performed += delegate { CheckEscape(); };
    }

    private void CheckEscape()
    {
        if (charCustom && charCustom.activeSelf) return;
        if (menuActive) Resume();
        else Pause();
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
        saveButton.interactable = EventManager.Instance.EventController.CanSaveLoad();
        var pauseTime = PauseWithoutScreen();
        if (pauseTime >= 0f) normalTimeScale = pauseTime;
        markedButton = -1;
        Up();
        menuActive = true;
        transform.SetActiveChildren(true);
        settingsPrefab.SetActive(false);
        savePrefab.SetActive(false);
    }

    public static float PauseWithoutScreen()
    {
        if (Time.timeScale <= 0)
        {
            return -1f;
        }
        var currentScale = Time.timeScale;
        Time.timeScale = 0;
        return currentScale;
    }

    public static void ResumeWithoutScreen(float normalTimeScale)
    {
        Time.timeScale = normalTimeScale;
    }

    public void Resume()
    {
        // dont use menuactive over false here
        menuActive = false;
        settingsPrefab.SetActive(false);
        savePrefab.SetActive(false);
        transform.SetActiveChildren(false);
        ResumeWithoutScreen(normalTimeScale);
    }

    public void Save()
    {
        myPanel.transform.SetActiveChildren(false);
        enabled = false;
        save.enabled = true;
        savePrefab.SetActive(true);
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
            levelController.calling();
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
            markerObj.transform.position = new Vector3(
                markerObj.transform.position.x,
                myChildButtons[markedButton].transform.position.y,
                markerObj.transform.position.z);
        }
    }

    protected void Down()
    {
        int t = markedButton;
        markedButton = Mathf.Min(
            myChildButtons.Count - 1, markedButton + 1);
        if (t != markedButton)
        {
            //move marker down
            markerObj.transform.position = new Vector3(
                markerObj.transform.position.x,
                myChildButtons[markedButton].transform.position.y,
                markerObj.transform.position.z);
        }
    }

    private void OnEnable()
    {
        settings.enabled = false;
    }
}
