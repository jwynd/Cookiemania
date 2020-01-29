using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class General_LevelTransition : MonoBehaviour
{
    public Dropdown LevelSelect; // Reference to the level select dropdown menu
    public GameObject[] DisableOnLevelChange; // List of desktop objects that should be disabled when in minigame
    private string loadedScene = null; // contains the name of the currently loaded minigame

    // in start we will add a listener so that we can call transition when a new entry is selected
    void start(){
        LevelSelect.onValueChanged.AddListener(delegate {
            transition(LevelSelect);
        });
    }
    
    // switch over the possible selections
    public void transition(Dropdown target){
        switch(target.value){
            case 0:
                returnDesktop();
                break;
            case 1:
                leaveDesktop("Jumper");
                break;
            default:
                Debug.LogError("General_LevelTransition: Selection not recognized"); // print error message
                #if UNITY_EDITOR // we only kick the user out in the editor
                UnityEditor.EditorApplication.isPlaying = false; // kick user out of session
                #endif
                break;

        }
    }

    // called when opening a new minigame, accepts a scene name
    private void leaveDesktop(string sceneName){
        foreach(GameObject g in DisableOnLevelChange){ // first disable unneeded objects in desktop
            g.SetActive(false);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive); // load the new scene additively
        loadedScene = sceneName; // set the loaded scene to a non-null value
    }

    // called when exiting a minigame and returning to the desktop
    private void returnDesktop(){
        if(loadedScene == null) return; // if loadedScene is null then we are not in a mini-game
        foreach(GameObject g in DisableOnLevelChange){
            if(g.tag == "DeactivateOnLoad") continue; // we don't want everything in desktop active at once
            g.SetActive(true);
        }
        // this operation is asynchronous, there is no guaruntee that it will finish running before execution continues
        SceneManager.UnloadSceneAsync(loadedScene);
        // If bugs arise, try commenting above, and uncommenting below
        // AsyncOperation async = SceneManger.UnloadSceneAsync(loadedScene);
        // while(!async.isDone); // Lock the scene in busy wait
        loadedScene = null;
    }

    // contains temporary controls to return to desktop, remove in final version
    void Update(){
        if(loadedScene == null) return;
        if(Input.GetKeyDown(KeyCode.Escape)){
            returnDesktop();
        }
    }
}