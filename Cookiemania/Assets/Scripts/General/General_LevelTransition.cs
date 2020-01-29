using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class General_LevelTransition : MonoBehaviour
{
    public Dropdown LevelSelect;
    public GameObject[] DisableOnLevelChange;
    private string loadedScene = null;
    void start(){
        LevelSelect.onValueChanged.AddListener(delegate {
            transition(LevelSelect);
        });
    }
    public void transition(Dropdown target){
        switch(target.value){
            case 0:
                returnDesktop();
                break;
            case 1:
                leaveDesktop("Jumper");
                break;
            default:
                Debug.LogError("Level Select: Selection not recognized");
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
                break;

        }
    }

    private void leaveDesktop(string sceneName){
        foreach(GameObject g in DisableOnLevelChange){
            g.SetActive(false);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        loadedScene = sceneName;
    }

    private void returnDesktop(){
        if(loadedScene == null) return;
        foreach(GameObject g in DisableOnLevelChange){
            if(g.tag == "DeactivateOnLoad") continue;
            g.SetActive(true);
        }
        SceneManager.UnloadSceneAsync(loadedScene);
        loadedScene = null;
    }

    void Update(){
        if(loadedScene == null) return;
        if(Input.GetKeyDown(KeyCode.Escape)){
            returnDesktop();
        }
    }
}