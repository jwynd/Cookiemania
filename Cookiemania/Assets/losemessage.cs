using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class losemessage : MonoBehaviour
{
    public static int Lcoins;
    string text;
    // Start is called before the first frame update
    void Start()
    {
        text = "Game Over!\n You made " + Lcoins.ToString() + " coins!";
        gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = text;
        PlayerData.Player.money += Lcoins;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
