using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperUIManager : MonoBehaviour
{
    #region variables
    public UnityEngine.UI.Slider slider;
    public float timeToNextScene = 1.0f;

    public static JumperUIManager Instance { get; private set; }

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
        slider.minValue = jm.player.transform.position.y;
        slider.maxValue = jm.GetHeightGoal();
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (jm.player != null)
        {
            slider.value = jm.player.transform.position.y;
        }
    }
    #region public
    public void GoodEnd()
    {
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
        if (jm.mainCamera != null)
        {
            //this method also triggers scene changing
            jm.mainCamera.PlayerDestroyed();
            StartCoroutine(TransitionScene(timeToNextScene));
        }
        Destroy(jm.player.gameObject);
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
