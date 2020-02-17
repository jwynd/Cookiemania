using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperUIManager : MonoBehaviour
{
    #region variables
    public UnityEngine.UI.Slider heightSlider;
    public UnityEngine.UI.Slider healthSlider;
    public GameObject endscreenCamera;
    public GameObject endscreenCanvas;
    public float timeToNextScene = 1.0f;

    public static JumperUIManager Instance { get; private set; }

    private const string sceneName = "Jumper";
    private bool gameRunning = true;
    private int coinsCollected = 0;
    private JumperManager jm;
    private General_LevelTransition levelController;
    private JumperUIText scoreText;

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
        scoreText = GetComponent<JumperUIText>();
        GameObject levelC = GameObject.Find("LevelController");
        if (levelC != null)
        {
            levelController = GameObject.Find("LevelController").GetComponent<General_LevelTransition>();
        }
    }

    void Start()
    {
        jm = JumperManager.Instance;
        heightSlider.minValue = jm.player.transform.position.y;
        heightSlider.maxValue = jm.GetHeightGoal();
        healthSlider.minValue = 0;
        healthSlider.maxValue = jm.player.GetMaxHealth();
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        //check if player alive
        if (gameRunning && jm.player != null)
        {
            heightSlider.value = jm.player.transform.position.y;
            healthSlider.value = jm.player.GetCurrentHealth();
            coinsCollected = (int)jm.player.GetCoinsCollected();
            scoreText.UpdateText(coinsCollected.ToString());
            if (heightSlider.value >= heightSlider.maxValue)
            {
                End(true);
            }
            else if (healthSlider.value <= healthSlider.minValue)
            {
                End(false);
            }
        }
        //game running and player is gone, badend
        else if (gameRunning)
        {
            End(false);
        }
    }
    #region public
    public void End(bool isGood)
    {
        gameRunning = false;
        jm.mainCamera.PlayerDestroyed();
        coinsCollected = (int)jm.player.GetCoinsCollected();
        //if this throws an error, need to stop whatever destroyed the player before this function
        Destroy(jm.player.gameObject);
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
        JumperEndscreen endUI = endscreenCanvas.GetComponent<JumperEndscreen>();
        endUI.UpdateText(coinsCollected.ToString());

        GetComponent<Canvas>().worldCamera.enabled = false;
        GetComponent<Canvas>().worldCamera.GetComponent<AudioListener>().enabled = false;
        endscreenCamera.GetComponent<Camera>().enabled = true;
        endscreenCamera.GetComponent<AudioListener>().enabled = true;
    }
    #endregion
}
