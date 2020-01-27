using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTransition : MonoBehaviour
{
    public Dropdown LevelSelect;
    //public string sceneName;
    void start(){
        LevelSelect.onValueChanged.AddListener(delegate {
            transition(LevelSelect);
        });
    }
    public void transition(Dropdown target){
        switch(target.value){
            case 0:
                break;
            case 1:
                SceneManager.LoadScene("SampleScene");
                break;
            default:
                print("Not Recognized");
                break;

        }
        //SceneManager.LoadScene(sceneName);
    }
}
