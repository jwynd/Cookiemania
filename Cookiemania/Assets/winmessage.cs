using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winmessage : MonoBehaviour
{
    public static int coins;
    string text;
    // Start is called before the first frame update
    void Start()
    {
        text = "Congratulations!\n You made " + coins.ToString() + " coins!";
        gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = text;
        PlayerData.Player.money += coins;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
