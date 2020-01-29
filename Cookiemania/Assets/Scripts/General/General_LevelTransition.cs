using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class General_LevelTransition : MonoBehaviour
{
    public Dropdown LevelSelect;
    private string loadedScene = null;
    void start(){
        LevelSelect.onValueChanged.AddListener(delegate {
            transition(LevelSelect);
        });
    }
    public void transition(Dropdown target){
        switch(target.value){
            case 0:
                if(loadedScene == null) break;
                SceneManager.UnloadSceneAsync(loadedScene);
                loadedScene = null;
                break;
            case 1:
                SceneManager.LoadScene("Jumper", LoadSceneMode.Additive);
                loadedScene = "Jumper";
                break;
            default:
                Debug.LogError("Level Select: Selection not recognized");
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
                break;

        }
    }
}
