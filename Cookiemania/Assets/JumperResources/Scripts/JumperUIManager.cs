using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperUIManager : MonoBehaviour
{
    #region variables
    public UnityEngine.UI.Slider heightSlider;
    public UnityEngine.UI.Slider healthSlider;
    public float timeToNextScene = 1.0f;

    public static JumperUIManager Instance { get; private set; }

    private bool gameRunning = true;
    private JumperManager jm;
    private General_LevelTransition levelController;

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
        if (jm.player != null)
        {
            heightSlider.value = jm.player.transform.position.y;
            healthSlider.value = jm.player.GetCurrentHealth();
            if (heightSlider.value >= heightSlider.maxValue)
            {
                GoodEnd();
            }
            if (healthSlider.value <= healthSlider.minValue)
            {
                BadEnd();
            }
        }
        else if (gameRunning)
        {
            BadEnd();
        }
    }
    #region public
    public void GoodEnd()
    {
        gameRunning = false;

        jm.mainCamera.PlayerDestroyed();
        //gotta do my transition here
        //then i can destroy my player
        //and maybe the platforms

        Destroy(jm.player.gameObject);

        StartCoroutine(TransitionScene(timeToNextScene));

        //transition screen to something else
        //dont need to destroy stuff but could?
    }
    public void BadEnd()
    {
        gameRunning = false;
        //this method also triggers scene changing
        jm.mainCamera.PlayerDestroyed();
        StartCoroutine(TransitionScene(timeToNextScene));
        
        Destroy(jm.player.gameObject);
        healthSlider.value = healthSlider.minValue;
    }
    #endregion

    #region coroutines
    private IEnumerator TransitionScene(float timer)
    {
        yield return new WaitForSeconds(timer);
        if (levelController)
        {
            levelController.returnDesktop();
        }
        Debug.LogWarning("No level transition object in game");
    }
    #endregion
}
