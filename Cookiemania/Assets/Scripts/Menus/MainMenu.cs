using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPrefab;
    public GameObject loadGamePrefab;
    public Button continueB;
    public Animator transitioning = null;

    private string continueFile = "";


    public void Start()
    {
        if (!PlayerPrefs.HasKey(PlayerDataStatics.P_PREFS_LAST_SAVED))
        {
            continueB.gameObject.SetActive(false);
        }
        else
        {
            continueFile = PlayerPrefs.GetString(PlayerDataStatics.P_PREFS_LAST_SAVED);
        }
    }

    public void Play()
    {
        //  SceneTransition(2);
        PlayerPrefs.DeleteKey(PlayerDataStatics.P_PREFS_LOAD);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Desktop", LoadSceneMode.Single);
    }

    public void Continue()
    {
        if (continueFile == "") return;
        PlayerPrefs.SetString(PlayerDataStatics.P_PREFS_LOAD, continueFile);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Desktop", LoadSceneMode.Single);
    }

    public void Settings()
    {
        // specific for main menu
        Transform c = GameObject.Find("Canvas").transform;
        List<GameObject> g = new List<GameObject>();

        foreach (Transform child in c)
        {
            g.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
        // needed to spawn the pause menu
        GameObject s = Instantiate(settingsPrefab);
        s.transform.SetParent(c, false);
        s.GetComponent<SettingsMenu>().activateOnDestroy = g;
        s.SetActive(true);
    }

    public void Load()
    {

    }

    public void SceneTransition(int version)
    {
        StartCoroutine(LoadTransition(version));
    }

    //Handles the animation
    IEnumerator LoadTransition(int version)
    {
        if (version == 1)
        {
            
            //Time.timeScale = 0;
            
            transitioning.SetTrigger("Start");

            //ideally we would load the scene here
            transitioning.ResetTrigger("Start");
            StartCoroutine("wait");
            // Time.timeScale = normalTimeScale;

        }
        else if (version == 2)
        {
          
            // Time.timeScale = 0;
      
            transitioning.SetBool("ExitScene", true);
            yield return new WaitForSeconds(2);
            //ideally we would load the scene here
            transitioning.SetBool("ExitScene", false);
            yield return new WaitForSeconds(2);//StartCoroutine("wait");
            //Time.timeScale = normalTimeScale;
        }
        else if (version == 3)
        {
          
            //Time.timeScale = 0;
           
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
    }
}
