using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    [SerializeField] private Text uiText;
    [SerializeField] private Text uiText2;
    private float mainTimer = 60f;

    private float timer;
    private bool count = true;
    private float hold;
    private float holds;
    private int dificulty = 0;

    private void Start()
    {
        if (PlayerData.Player != null)
        {
            dificulty = PlayerData.Player.spacelvl;
        }
        if (dificulty == 0)
        {
            mainTimer = 60f;
        } else if (dificulty == 1)
        {
            mainTimer = 180f;
        } else if (dificulty == 2)
        {
            mainTimer = 300f;
        }
        else
        {
            mainTimer = 10f;
        }
        timer = mainTimer;
        hold = 3;
        uiText.text = hold.ToString();
    }
    private void Update()
    {
        if (timer >= 0.0f && count)
        {
            timer -= Time.deltaTime;
            hold = Mathf.Floor(timer / 60);
            uiText.text = hold.ToString();
            holds = Mathf.RoundToInt(timer % 60);
            if (holds == 60 || holds == 0)
            {
                holds = 00f;
                uiText2.text = holds.ToString() + "0";
            } else if (timer <= 9.5f)
            {
                uiText2.text = "0" + holds.ToString();
            } else
            {
                uiText2.text = holds.ToString();
            }


        }
        else if (timer <= 0.0f && timer > -1)
        {
            count = false;
            uiText.text = "0.00";
            timer = 0.0f;
            FindObjectOfType<spaceManager>().winGame();
            timer = -2f;
        }
        else
        {
            Debug.Log("Space Game is Won");
        }
    }
}
