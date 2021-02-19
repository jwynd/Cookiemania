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
    public JumperBackgroundController endscreenBG;
    public float timeToNextScene = 2.5f;
    public TextMeshProUGUI scoreRef;
    public TextMeshProUGUI tutorialRef;

    public static JumperManagerUI Instance { get; private set; }

    private const string sceneName = "Jumper";
    private bool endingGame = false;
    private int coinsCollected = 0;
    private JumperManagerGame jm;


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
        endscreenCanvas.SetActive(false);
        endscreenBG = endscreenCanvas.GetComponent<JumperBackgroundController>();
    }

   
    void Start()
    {
        jm = JumperManagerGame.Instance;
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
            scoreRef.text = coinsCollected.ToString();
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
        endscreenCanvas.SetActive(true);
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
            endscreenBG.SetNight(false);
            coinsCollected = (int)(coinsCollected * 1.5f);
            endUI.UpdateText("LEVEL COMPLETE!<br>you made " + coinsCollected.ToString());

        }
        else
        {
            endscreenBG.SetNight(true);
            endUI.UpdateText("Game Over<br>you made " + coinsCollected.ToString());
        }
        
        GetComponent<Canvas>().worldCamera.enabled = false;
        GetComponent<Canvas>().worldCamera.GetComponent<AudioListener>().enabled = false;
        endscreenCamera.GetComponent<Camera>().enabled = true;
        endscreenCamera.GetComponent<AudioListener>().enabled = true;
        endscreenCanvas.SetActive(true);
        PlayerData.Player?.AddMoney(coinsCollected);
    }
    #endregion
}
