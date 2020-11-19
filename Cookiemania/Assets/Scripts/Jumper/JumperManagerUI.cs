using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class JumperManagerUI : MonoBehaviour
{
    #region variables
    public UnityEngine.UI.Slider heightSlider;
    public UnityEngine.UI.Slider healthSlider;
    public GameObject endscreenCamera;
    public GameObject endscreenCanvas;
    public float timeToNextScene = 2.5f;
    public TextMeshProUGUI scoreRef;
    public TextMeshProUGUI tutorialRef;
   


    public static JumperManagerUI Instance { get; private set; }

    private const string sceneName = "Jumper";
    private bool endingGame = false;
    private int coinsCollected = 0;
    private JumperManagerGame jm;
    private General_LevelTransition levelController;
    private JumperGeneralText scoreText;
    private JumperGeneralText tutorialText;
    private bool tutorialActive = false;
    private bool needNextLine;
    private JumperStoryFramework storyfw;
    private bool listeningToAxis;
    private string axis;
    private float tutorialTimer;
    private float minTutorialTimer = 0.35f;

    #endregion

    #region startup
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        endscreenCamera.GetComponent<Camera>().enabled = false;
        endscreenCamera.GetComponent<AudioListener>().enabled = false;
        scoreText = scoreRef.gameObject.GetComponent<JumperGeneralText>();

        tutorialText = tutorialRef.gameObject.GetComponent<JumperGeneralText>();
        
    }

   
    void Start()
    {
        jm = JumperManagerGame.Instance;
        storyfw = JumperStoryFramework.Instance;
        levelController = General_LevelTransition.Instance;
        heightSlider.minValue = jm.Player.transform.position.y;
        heightSlider.maxValue = jm.GetHeightGoal();
        healthSlider.minValue = 0;
        healthSlider.maxValue = jm.Player.GetMaxHealth();
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        //check if player alive
        if (jm.Player != null)
        {
            heightSlider.value = jm.Player.transform.position.y;
            healthSlider.value = jm.Player.GetCurrentHealth();
            coinsCollected = (int)jm.Player.GetCoinsCollected();
            scoreText.UpdateText(coinsCollected.ToString());
        }
        TutorialUpdate();
    }

    protected void TutorialUpdate()
    {
        if (!tutorialActive)
        {
            return;
        }
        if (needNextLine)
        {
            needNextLine = false;
            System.Tuple<string, string> textNAxis = storyfw.GetNextTutorialLine();
            if (textNAxis == null)
            {
                tutorialActive = false;
                tutorialText.UpdateText("");
                return;
            }
            else if (storyfw.IsAnAxis(textNAxis.Item2))
            {
                listeningToAxis = true;
                axis = textNAxis.Item2;
            }
            else
            {
                listeningToAxis = false;
                tutorialTimer = float.Parse(textNAxis.Item2);
                Debug.Log(tutorialTimer.ToString());
            }
            tutorialText.UpdateText(textNAxis.Item1);
        }  
        if (listeningToAxis)
        {
            CheckPlayerInput(axis);
        }
        else
        {
            tutorialTimer -= Time.deltaTime;
            if (tutorialTimer < 0)
            {
                needNextLine = true;
            }
        }
    }

    private void CheckPlayerInput(string axis)
    {
        
        //switch did not work ??
        if (axis == nameof(jm.Player.Input.Horizontal))
        {
            if (jm.Player.Input.Horizontal != 0)
            {
                listeningToAxis = false;
                tutorialTimer = minTutorialTimer;
            }       
        }
        //should only be checking this when we have an item to throw tbh
        else if (axis == nameof(jm.Player.Input.Throw))
        {
            if (jm.Player.Input.Throw != 0)
            {
                listeningToAxis = false;
                tutorialTimer = minTutorialTimer;
            }
        }
        else if (axis == nameof(jm.Player.Input.Jump))
        {
            if (jm.Player.Input.Jump != 0)
            {
                listeningToAxis = false;
                tutorialTimer = minTutorialTimer;
            }
        }
        else if (axis == nameof(jm.Player.Input.Pickup))
        {
            if (jm.Player.HasThrowable())
            {
                listeningToAxis = false;
                tutorialTimer = minTutorialTimer;
            }
        }
    }
    #region public
    public void End(bool isGood, bool runSequence = true, Transform target = null)
    {
        //ensuring this only gets run once
        if (endingGame) { return; }
        
        jm.MainCam.PlayerDestroyed();
        endingGame = true;
        if (!isGood && runSequence) 
            jm.Player.RunDeathSequence();
        else if (runSequence)
            jm.Player.RunVictorySequence();
        if (target != null)
        {
            jm.MainCam.ZoomInToTarget(target);
        }
        else
        {
            jm.MainCam.ZoomIn();
        }
        StartCoroutine(SwitchCamera(timeToNextScene, isGood));
    }

    public void Retry()
    {
        General_LevelTransition trans = General_LevelTransition.Instance;
        if (trans)
        {
            trans.calling();
            trans.LDesk(sceneName);
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void ReturnToDesktop()
    {
        General_LevelTransition trans = General_LevelTransition.Instance;
        if (trans)
        {
            trans.calling();
            return;
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.LogWarning("No level transition object in game");

        Application.Quit();
#endif
    }
    #endregion

    #region coroutines
    private IEnumerator SwitchCamera(float timer, bool isGoodEnd)
    {
        yield return new WaitForSeconds(timer);
        coinsCollected = (int)jm.Player.GetCoinsCollected();
        //if this throws an error, need to stop whatever destroyed the player before this function
        Destroy(jm.Player.gameObject);
        JumperTextAdvanced endUI = endscreenCanvas.GetComponent<JumperTextAdvanced>();
        if (isGoodEnd)
        {
            coinsCollected += (int)JumperManagerGame.Instance.GetLevelReward();
            endUI.UpdateText("LEVEL COMPLETE!<br>you made " + coinsCollected.ToString());

        }
        else
        {
            endUI.UpdateText("Game Over<br>you made " + coinsCollected.ToString());
        }

        GetComponent<Canvas>().worldCamera.enabled = false;
        GetComponent<Canvas>().worldCamera.GetComponent<AudioListener>().enabled = false;
        endscreenCamera.GetComponent<Camera>().enabled = true;
        endscreenCamera.GetComponent<AudioListener>().enabled = true;
    }
    #endregion
}
