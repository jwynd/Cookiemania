using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public GameObject settingsPrefab;
    
    public void Play(){
        SceneManager.LoadScene("Desktop", LoadSceneMode.Single);
    }

    public void Settings(){
        // specific for main menu
        Transform c = GameObject.Find("Canvas").transform;
        List<GameObject> g = new List<GameObject>();
        
        foreach(Transform child in c){
            g.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
        // needed to spawn the pause menu
        GameObject s = Instantiate(settingsPrefab);
        s.transform.parent = c;
        s.transform.position = new Vector3(Screen.width*0.5f, Screen.height*0.5f, 0);
        s.GetComponent<SettingsMenu>().activateOnDestroy = g.ToArray();
        s.SetActive(true);
    }
}
