using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spaceManager : MonoBehaviour
{
    public GameObject win;
    public GameObject lose;
    
    public void EndGame()
    {
        Debug.Log("GAME OVER");
        lose.SetActive(true);
        Time.timeScale = 0f;
    }

    public void winGame()
    {
        Debug.Log("Win Game");
        win.SetActive(true);
        Time.timeScale = 0f;
    }

    public void leaveGame()
    {
        Time.timeScale = 1;
        if (!General_LevelTransition.Instance)
        {
            return;
        }
        if (General_LevelTransition.Instance.LoadedScene == null)
        {
            return;
        }
        General_LevelTransition.Instance.SceneTransition(2);
        General_LevelTransition.Instance.calling();
    }
}
