using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class General_LevelTransition : MonoBehaviour
{
    public Dropdown LevelSelect; // Reference to the level select dropdown menu
    public List<GameObject> DisableOnLevelChange; // List of desktop objects that should be disabled when in minigame
    public GameObject pauseMenuPrefab;
    private GameObject gamePauseMenu;
    public string LoadedScene { get; protected set; } // contains the name of the currently loaded minigame
    public static General_LevelTransition Instance { get; protected set; }
    public Animator transitioning;
    private float transitionTime = 3f;
    protected float normalTimeScale;
    public AnimationClip animEnter;
    public AnimationClip animExit;
    public GameObject hometab;
    private bool loadingMinigame = false;

    // public delegate void OnLevelTransiion();
    // public event OnLevelTransiion onBegin;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        gamePauseMenu = Instantiate(pauseMenuPrefab);
        //gamePauseMenu.SetActive(false);
    }

    // in start we will add a listener so that we can call transition when a new entry is selected
    //this isnt working, which is good cuz it double loads the scene
    void start()
    {

        SceneTransition(1);
        LevelSelect.onValueChanged.AddListener(delegate
        {
            transition(LevelSelect);
        });
    }

    // switch over the possible selections
    public void transition(Dropdown target)
    {
        switch (target.value)
        {
            case 0:
                StartCoroutine(returnDesktop());
                break;
            case 1:
                //StartCoroutine("wait");
                StartCoroutine(leaveDesktop("Jumper"));
                break;
            case 2:
                // StartCoroutine("wait");
                StartCoroutine(leaveDesktop("Spacemini"));
                break;
            case 3:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
            default:
                Debug.LogError("General_LevelTransition: Selection not recognized"); // print error message
#if UNITY_EDITOR // we only kick the user out in the editor
                UnityEditor.EditorApplication.isPlaying = false; // kick user out of session
#endif
                break;

        }
    }

    public void ToMinigame(string scene)
    {
        if (loadingMinigame)
            return;
        loadingMinigame = true;
        StartCoroutine(leaveDesktop(scene));
    }

    // called when opening a new minigame, accepts a scene name
    IEnumerator leaveDesktop(string sceneName)
    {
        SceneTransition(2);
        yield return StartCoroutine(wait());
        foreach (GameObject g in DisableOnLevelChange)
        { // first disable unneeded objects in desktop
            g.SetActive(false);
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive); // load the new scene additively
        LoadedScene = sceneName; // set the loaded scene to a non-null value
        Debug.Log("scene set " + LoadedScene);
        loadingMinigame = false;
        //gamePauseMenu.SetActive(true);
    }

    // called when exiting a minigame and returning to the desktop
    IEnumerator returnDesktop()
    {

        SceneTransition(2);
        yield return StartCoroutine(wait());
        if (LoadedScene == null) yield break; // if loadedScene is null then we are not in a mini-game
        foreach (GameObject g in DisableOnLevelChange)
        {
            if (g.CompareTag("DeactivateOnLoad")) continue; // we don't want everything in desktop active at once
            g.SetActive(true);
        }
        // this operation is asynchronous, there is no guaruntee that it will finish running before execution continues

        //SceneManager.UnloadSceneAsync(loadedScene);
        // If bugs arise, try commenting above, and uncommenting below
        AsyncOperation async = SceneManager.UnloadSceneAsync(LoadedScene);
        // while(!async.isDone); // Lock the scene in busy wait
        LoadedScene = null;
        if (LevelSelect)
            LevelSelect.value = 0;
        hometab.GetComponent<General_TabButton>().click();
    }
    // Calls and wraps the function to play animation for scene transition
    public void SceneTransition(int version)
    {
        StartCoroutine(LoadTransition(version));
    }

    //Handles the animation
    IEnumerator LoadTransition(int version)
    {
        if (version == 1)
        {
            normalTimeScale = Time.timeScale;
            //Time.timeScale = 0;
            gamePauseMenu.SetActive(false);
            transitioning.SetTrigger("Start");

            //ideally we would load the scene here
            transitioning.ResetTrigger("Start");
            StartCoroutine("wait");
            // Time.timeScale = normalTimeScale;

        }
        else if (version == 2)
        {
            normalTimeScale = Time.timeScale;
            // Time.timeScale = 0;
            gamePauseMenu.SetActive(false);
            transitioning.SetBool("ExitScene", true);
            yield return new WaitForSeconds(2);
            //ideally we would load the scene here
            transitioning.SetBool("ExitScene", false);
            yield return new WaitForSeconds(2);//StartCoroutine("wait");
            //Time.timeScale = normalTimeScale;
        }
        else if (version == 3)
        {
            normalTimeScale = Time.timeScale;
            //Time.timeScale = 0;
            gamePauseMenu.SetActive(false);
            yield return new WaitForSeconds(1);
            //ideally we would load the scene here
            transitioning.SetBool("ExitScene", false);
            yield return new WaitForSeconds(1);
            //Time.timeScale = normalTimeScale;
        }

    }
    // allows couroutines to pause for seconds
    IEnumerator wait()
    {
        yield return new WaitForSeconds(2);
        gamePauseMenu.SetActive(true);
    }

    //This function acts as a bypass for the coroutine return desktop in other scripts
    public void calling()
    {
        Debug.Log(LoadedScene);
        if (LoadedScene == null)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        StartCoroutine(returnDesktop());
    }

    //This function acts as a bypass for the coroutine leave desktop in other scripts
    public void LDesk(string scenename)
    {
        StartCoroutine(leaveDesktop(scenename));
    }
}