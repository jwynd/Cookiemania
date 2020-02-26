using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperManagerUI : MonoBehaviour
{
    #region variables
    public UnityEngine.UI.Slider heightSlider;
    public UnityEngine.UI.Slider healthSlider;
    public GameObject endscreenCamera;
    public GameObject endscreenCanvas;
    public float timeToNextScene = 2.5f;
    public float maxFallDistance = 15f;
    public static JumperManagerUI Instance { get; private set; }

    private const string sceneName = "Jumper";
    private bool endingGame = false;
    private int coinsCollected = 0;
    private JumperManagerGame jm;
    private General_LevelTransition levelController;
    private JumperGeneralText scoreText;
    private float maxHeightReached;

    #endregion

    #region startup
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        endscreenCamera.GetComponent<Camera>().enabled = false;
        endscreenCamera.GetComponent<AudioListener>().enabled = false;
        scoreText = GetComponent<JumperGeneralText>();
        GameObject levelC = GameObject.Find("LevelController");
        if (levelC != null)
        {
            levelController = GameObject.Find("LevelController").GetComponent<General_LevelTransition>();
        }
    }

    void Start()
    {
        jm = JumperManagerGame.Instance;
        heightSlider.minValue = jm.player.transform.position.y;
        heightSlider.maxValue = jm.GetHeightGoal();
        maxHeightReached = jm.player.transform.position.y;
        healthSlider.minValue = 0;
        healthSlider.maxValue = jm.player.GetMaxHealth();
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        //check if player alive
        if (jm.player != null)
        {
            heightSlider.value = jm.player.transform.position.y;
            healthSlider.value = jm.player.GetCurrentHealth();
            coinsCollected = (int)jm.player.GetCoinsCollected();
            scoreText.UpdateText(coinsCollected.ToString());
        }
    }
    #region public
    public void End(bool isGood, bool runSequence = true, Transform target = null)
    {
        //ensuring this only gets run once
        if (endingGame) { return; }
        
        jm.mainCamera.PlayerDestroyed();
        endingGame = true;
        if (!isGood && runSequence) 
            jm.player.RunDeathSequence();
        else if (runSequence)
            jm.player.RunVictorySequence();
        if (target != null)
        {
            jm.mainCamera.ZoomInToTarget(target);
        }
        else
        {
            jm.mainCamera.ZoomIn();
        }
        StartCoroutine(SwitchCamera(timeToNextScene, isGood));
    }

    public void Retry()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void ReturnToDesktop()
    {
        if (levelController)
        {
            levelController.returnDesktop();
        }
        Debug.LogWarning("No level transition object in game");
    }
    #endregion

    #region coroutines
    private IEnumerator SwitchCamera(float timer, bool isGoodEnd)
    {
        yield return new WaitForSeconds(timer);
        coinsCollected = (int)jm.player.GetCoinsCollected();
        //if this throws an error, need to stop whatever destroyed the player before this function
        Destroy(jm.player.gameObject);
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
