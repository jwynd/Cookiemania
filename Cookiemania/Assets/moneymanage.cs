using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class moneymanage : MonoBehaviour
{
    string hold;
    void Start()
    {
        hold = gameObject.GetComponent<Text>().text;
        gameObject.GetComponent<Text>().text += PlayerData.Player.money;
    }

    // Update is called once per frame
    public void moneyupdate()
    {
        gameObject.GetComponent<Text>().text = hold + PlayerData.Player.money; 
    }
}
